/* Read a Bitmap File */

#include "BMP.h"

/* Read header File */
sImageHeader readImage(char* fileName) {
	sImageHeader sImHead;
	int i;
	FILE *bmpInput;
	
	if ((bmpInput = fopen(fileName, "rb")) == NULL)
	 {
		 printf("Can not read BMP file");
		 exit(0);
	 } /* Read BMP input file */

	 fseek(bmpInput, 0L, SEEK_SET);   /* File pointer at byte #0 */

	 /* Signature of the File BM = BMP at byte # 0*/
	 for(i=0; i<2; i++)
	 {
		 sImHead.signature[i] = (char)getImageInfo(bmpInput, i, 1);
	 }
	 if ((sImHead.signature[0] == 'B') && (sImHead.signature[1] == 'M')) printf("It is verified that the Image is in Bitmap format\n");
	 else
	 {
		 printf("The image is not a bitmap format, quitting....\n");
		 exit(0);
	 }

	 /* Read BMP bits/pixel at byte #28 */
	 sImHead.nBits = (int)getImageInfo(bmpInput, 28, 2);
	 //printf("The Image is \t%d-bit\n", sImHead.nBits);

	 /* Position of First Raster Data at byte # 10*/
	 sImHead.rasterOffset = (int)getImageInfo(bmpInput, 10, 4);
	 //printf("The beginning of the Raster Data \nis at \t\t%d byte\n", sImHead.rasterOffset);

	 /* Read BMP file size at byte # 2 */
	 sImHead.fileSize = getImageInfo(bmpInput, 2, 4);
	 //printf("File size is \t%ld byte\n", sImHead.fileSize);

	  /* Read BMP width at byte #18 */
	 sImHead.nCols = (int)getImageInfo(bmpInput, 18, 4);
	 //printf("Width: \t\t%d\n", sImHead.nCols);

	 /* Read BMP height at byte #22 */
	 sImHead.nRows = (int)getImageInfo(bmpInput, 22, 4);
	 //printf("Height: \t%d\n", sImHead.nRows);

	 /* # of Pixels in a meter in x direction at byte # 38 */
	 sImHead.xpixpeRm = (int)getImageInfo(bmpInput, 38, 4);
	 //printf("Image has \t%d pixels per m in x-dir.\n", sImHead.xpixpeRm);

	 /* # of Pixels in a meter in y direction at byte # 42 */
	 sImHead.ypixpeRm = getImageInfo(bmpInput, 42, 4);
	 //printf("Image has \t%d pixels per m in y-dir.\n", sImHead.ypixpeRm);

	 /* Read number of colors at byte #46 */
	 sImHead.nColors = pow(2L,sImHead.nBits);
	 //printf("There are \t%ld number of Colors \n", sImHead.nColors);
	 
	 /* Read Red channel bit mask #54 */
	 sImHead.RedMask = getImageInfo(bmpInput, 54,4);
	 //printf("Red Mask\t%x \n", sImHead.RedMask);
	 	 
	 /* Read Green channel bit mask #58 */
	 sImHead.GreenMask = getImageInfo(bmpInput, 58,4);
	 //printf("Green Mask\t%x \n", sImHead.GreenMask);
	 
	 /* Read Blue channel bit mask #62 */
	 sImHead.BlueMask = getImageInfo(bmpInput, 62,4);
	 //printf("Blue Mask\t%x \n", sImHead.BlueMask);
	 
	 sImHead.vectorSize = (long)((long)sImHead.nCols*(long)sImHead.nRows);
	 //printf("Vector Size is \t%ld\n", sImHead.vectorSize);

	 fclose (bmpInput);
	
	return sImHead;
}

/* Read file a specific offset */
long getImageInfo(FILE* inputFile, long offset, int numberOfChars)
 {

  unsigned char   *ptrC;
  long            value=0L;
  unsigned char   dummy;
  int             i;

  dummy = '0'; ptrC = &dummy;

  fseek(inputFile, offset, SEEK_SET);

  for(i=1; i<=numberOfChars; i++)
  {
     fread(ptrC, sizeof(char), 1, inputFile);

     value = (long)(value + (*ptrC)*(pow(256, (i-1))));
  }

  return(value);

 } /* end of getImageInfo */

 
 /* Return the pad row size */
 int getPaddingOfMatrix(int x , int nbBytesByPixel) {
 	
 	
 	int padd = x * nbBytesByPixel;
 	
 	while( padd > 4 ) {
 		padd -= 4;
 	}
 	
 	return (4-padd)%4;
 }
 
 void loadMatric(char *fileName, unsigned long **data , int x , int y , int nbBytesByPixel, int offset) {
 	
	 /* go to start of pixels data*/
 	FILE *file = fopen( fileName , "rb");
 	fseek(file, offset , 0);
 	
 	/* init variables*/
 	int padding = getPaddingOfMatrix(x, nbBytesByPixel);
 	uint8_t r,v,b;
 	int remainingX = 0;
 	int remainingY = 0;
 	
 	/* read file */
 	while(remainingY < y) {
 		
 		remainingX = 0;
 		while(remainingX < x) {
 			
 			/*
 			 * Check for little endian, so reverse bytes : ABVR and no RVBA
 			 * unComment Alpha for 32 bits picture
 			 * 
 			 * The lines are reverse
 			 * Pad row size  to a multiple of 4 Bytes
 			 * 
 			 * DATA structure :		h = nbLines 	w = nbCols
 			 * -----------------------------------------------------
 			 *  0,h-1	|  1,h-1	|  2,h-1	| ....	|  w-1,h-1	| Padding
 			 * -----------------------------------------------------
 			 *	0,h-2	|  1,h-2	|  2,h-2	| ....	|  w-1,h-2	| Padding
 			 * -----------------------------------------------------
 			 *  ...			...			...		  ....		...
 			 * -----------------------------------------------------
 			 *	0,0		|  1,0		|  2,0		| ....	|  w-1,0	| Padding
 			 * -----------------------------------------------------
 			 */
 			
 			unsigned long tmp= 0;
 			//Alpha
 			//fgetc(file);
 			//R
 			b = fgetc(file);
 			//G
 			v = fgetc(file);
 			//B
 			r = fgetc(file);
 			
 			/* build pixel */
 			tmp = tmp<<8; tmp +=(unsigned long)r;  
 			tmp = tmp<<8; tmp +=(unsigned long)v; 
 			tmp = tmp<<8; tmp +=(unsigned long)b; 
 			
 			/* store it in DATA[][]*/
 			data[y-remainingY-1][remainingX] = tmp;
 			
 	       remainingX ++;
 		}
 		
 		/* check Paddind at end of each row */ 
 		int paddingRemaining = padding;
 		while(paddingRemaining > 0){
 			fgetc(file);
 			paddingRemaining --;
 		}
 		
 		remainingY ++;
 	}
 }
 
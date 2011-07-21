#ifndef BMP_H_
#define BMP_H_

#include<stdio.h>
#include<stdlib.h>
#include<math.h>
#include <stdint.h>

/* Structures */
typedef struct
{
int rows; int cols; unsigned char* data;
} sImage;

typedef struct
{
	 char		 	signature[2];	     /* Signature of the Image File BM = BMP */
	 int            nRows, nCols;        /* Row and Column size of the Image */
	 int            xpixpeRm, ypixpeRm;  /* Pixel/m */
	 long           nColors;             /* BMP number of colors */
	 long           fileSize;            /* BMP file size */
	 long           vectorSize;          /* BMP's raster data in number of bytes */
	 int		 	nBits;	             /* # of BIts per Pixel */
	 int		 	rasterOffset;        /* Beginning of the Raster Data */
	 int 			ICCProfileSize;		 /* The ICC Profile Size (footer)*/
	 long 			RedMask;			
	 long 			GreenMask;
	 long 			BlueMask;
} sImageHeader;

typedef struct 
{
	short int r;
	short int v;
	short int b;
} Color;

/* Functions */
long getImageInfo(FILE*, long, int);
sImageHeader readImage(char*);
void loadMatric(char *, unsigned long ** , int , int , int , int );
int getPaddingOfMatrix(int , int );

#endif /*BMP_H_*/

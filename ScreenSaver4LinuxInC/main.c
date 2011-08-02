
#include <X11/Xlib.h> 

#include <time.h>
#include <unistd.h> 
#include <signal.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h> 

#include "vroot.h"
#include "BMP.h"

void reverse(char s[])
 {
     int i, j;
     char c;
 
     for (i = 0, j = strlen(s)-1; i<j; i++, j--) {
         c = s[i];
         s[i] = s[j];
         s[j] = c;
     }
 }

void itoa(int n, char s[])
 {
     int i, sign;
 
     if ((sign = n) < 0)  /* record sign */
         n = -n;          /* make n positive */
     i = 0;
     do {       /* generate digits in reverse order */
         s[i++] = n % 10 + '0';   /* get next digit */
     } while ((n /= 10) > 0);     /* delete it */
     if (sign < 0)
         s[i++] = '-';
     s[i] = '\0';
     reverse(s);
 }

/*
 * Handler to detect end of screensaver ; catch SIGTERM
 */
void handler(int signum){
    printf("sigterm catched\n");
    
    remove("/tmp/ScreenSaverData.txt");
    remove("/tmp/test.png");
    
    //Send "stopJVM" signal to the daemon
    system("python /usr/bin/PAAgent/client_daemon.py stopJVM");
    exit(0);
}


int main() 
{ 
        FILE *f;

        if (f = fopen( "/tmp/ScreenSaver.png" ,"r")) {

            remove("/tmp/ScreenSaver.png");
        }
        if (f = fopen( "/tmp/ScreenSaverData.txt" ,"r")) {

            remove("/tmp/ScreenSaverData.txt");
        }


	//Send "startJVM" signal to the daemon
        system("python /usr/bin/PAAgent/client_daemon.py startJVM");
	signal( SIGTERM, handler );
	
	/* Graphicals datas */
	Display * display; 
	int screen_number, depth, bitmap_pad; 
	unsigned long white; 
	unsigned long black; 
	Window root, win; 
	GC gc; 
	Visual * visual; 
	XImage * image; 
	unsigned int image_width, image_height; 
	unsigned long image_byte_size;
	Pixmap pixmap; 
	/* ************** */

	display = XOpenDisplay (NULL); 
	if (! display) 
	{ 
		return 1; 
	} 

	/* Initialisation*/
	screen_number = DefaultScreen (display); 
	depth = DefaultDepth (display, screen_number); 
	visual = DefaultVisual (display, screen_number); 
	gc = DefaultGC (display, screen_number); 
	bitmap_pad = BitmapPad (display); 
	white = WhitePixel (display, screen_number); 
	black = BlackPixel (display, screen_number); 
	root = DefaultRootWindow (display); 
	
	image_width = DisplayWidth(display, screen_number);
	image_height = DisplayHeight(display, screen_number);

        char command[] = "java -jar /usr/bin/PAAgent/FullScreenSaver.jar /tmp/ScreenSaver.png /tmp/ScreenSaverData.txt ";
        char space[] = " ";
        char w[5] = "" , h[5] = ""  , tmp[10] = "" ;

        itoa(image_width , w);
        strcat(tmp , w);
        strcat(tmp , space);
        itoa(image_height , h);
        strcat(tmp , h);
        strcat(command , tmp);
        system(command);
       

        /* Read file */
	int i,k;
        char *fileName = "/tmp/ScreenSaver.png";
	sImageHeader sImHead = readImage(fileName);
	
	/* The picture DATA */
	unsigned long **image_data = (unsigned long **) malloc (sImHead.nRows * sizeof(unsigned long *) );
	for(i=0 ; i < sImHead.nRows ; ++i) {
		image_data[i] = (unsigned long *) malloc (sImHead.nCols * sizeof(unsigned long) );
	}
	/* load BMP file */
	loadMatric(fileName , image_data , sImHead.nCols , sImHead.nRows , 3 , sImHead.rasterOffset);
	/* ************* */

        

	/* Create simple window to receive picture */
	win = XCreateSimpleWindow (display, 
	root, 
	0,0, 
	DisplayWidth(display,screen_number), DisplayHeight(display, screen_number), 
	0, black, 
	black); 

	/* Map window on display */
	XMapWindow (display, win); 

	/* Generate the pixmap structure */
	image = XCreateImage (display, visual, depth, ZPixmap , 0, NULL, image_width, image_height, bitmap_pad, 0); 
	image_byte_size = (long)((long)DisplayWidth(display,screen_number) * (long)DisplayHeight(display, screen_number) * (long)(image->bits_per_pixel) / 8); 
	image->data = (char*)malloc(image_byte_size * sizeof(char));
	pixmap = XCreatePixmap (display, root, image_width, image_height, depth); 
	
	/* draw on the pixmap */
	for(k = 0 ; k < sImHead.nRows ; ++k) {
		for (i = 0 ; i < sImHead.nCols ; ++i) {
			XPutPixel( image , i , k , image_data[k][i]);
		}
	}
	
	int posImageX = (DisplayWidth(display , 0) - image_width)/2;
	int posImageY = (DisplayHeight(display , 0) - image_height)/2;

	while(1)
	{ 
		// Display it on the screen
		XPutImage (display, pixmap, gc, image, 0, 0, 0,0, image_width, image_height); 
		XCopyArea (display, pixmap, win, gc, 0,0, image_width, image_height, posImageX , posImageY);
		
		system(command);
		//sleep(1);
		
		/* load BMP file */
		loadMatric(fileName , image_data , sImHead.nCols , sImHead.nRows , 3 , sImHead.rasterOffset);
		/* ************* */
		
		/* draw on the pixmap */
		for(k = 0 ; k < sImHead.nRows ; ++k) {
			for (i = 0 ; i < sImHead.nCols ; ++i) {
				XPutPixel( image , i , k , image_data[k][i]);
			}
		}
	} 

	
	/* Free memory */
	XDestroyImage (image); 
	XCloseDisplay (display); 

	for(i=0 ; i < sImHead.nRows ; ++i) {
		free(image_data[i]);
	}
	free(image_data);
	
	return 0; 
} 

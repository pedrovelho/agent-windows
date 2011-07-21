CC=gcc
CFLAGS=-I.

main.exe: main.o BMP.o
	$(CC) -Wall -o main.exe main.o BMP.o -lX11 -lm

main.o: main.c BMP.c BMP.h
	$(CC) -Wall -o main.o -c main.c -lX11
	
BMP.o: BMP.c BMP.h
	$(CC) -Wall -o BMP.o -c BMP.c -lm
	
clean:
	rm -rf *.exe *.o
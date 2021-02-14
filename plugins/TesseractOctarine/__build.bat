md build 2>nul
rc /fo build/TesseractOctarine.res TesseractOctarine.rc
csc /out:build/TesseractOctarine.dll TesseractOctarine.cs /r:lib/tesseract.dll /r:../../build/octarine.exe /optimize /target:library /platform:anycpu  /win32res:build/TesseractOctarine.res
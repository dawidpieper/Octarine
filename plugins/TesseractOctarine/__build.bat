md build 2>nul
rc /fo build/TesseractOctarine.res TesseractOctarine.rc
csc /out:build/Tesseract.Octarine.dll TesseractOctarine.cs /r:lib/tesseract.dll /r:../../build/octarine.exe /optimize /target:library /platform:anycpu  /win32res:build/TesseractOctarine.res
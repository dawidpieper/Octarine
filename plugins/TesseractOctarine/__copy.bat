md ..\..\build 2>nul
md ..\..\build\plugins 2>nul
copy build\Tesseract.Octarine.dll ..\..\build\plugins\Tesseract.Octarine.dll
md ..\..\build\plugins 2>nul
md ..\..\build\plugins\Tesseract.Octarine\x86 2>nul
md ..\..\build\plugins\Tesseract.Octarine\x64 2>nul
copy /y lib\*.dll ..\..\build\plugins
copy /y x86\*.dll ..\..\build\plugins\Tesseract.Octarine\x86
copy /y x64\*.dll ..\..\build\plugins\Tesseract.Octarine\x64
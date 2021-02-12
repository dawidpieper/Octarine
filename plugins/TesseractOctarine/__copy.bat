md ..\..\build 2>nul
md ..\..\build\plugins 2>nul
copy build\TesseractOctarine.dll ..\..\build\plugins\TesseractOctarine.dll
md ..\..\build\plugins\dependencies 2>nul
md ..\..\build\plugins\dependencies\x86 2>nul
md ..\..\build\plugins\dependencies\x64 2>nul
md ..\..\build\plugins\dependencies\tessdata 2>nul
copy /y lib\*.dll ..\..\build\plugins\dependencies
copy /y x86\*.dll ..\..\build\plugins\dependencies\x86
copy /y x64\*.dll ..\..\build\plugins\dependencies\x64
copy /y tessdata\*.ttf ..\..\build\plugins\dependencies\tessdata\*
copy /y tessdata\*.traineddata ..\..\build\plugins\dependencies\tessdata\*
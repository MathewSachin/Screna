![Logo](Screna.png)
# Screna

[![Join the chat at https://gitter.im/MathewSachin/Screna](https://badges.gitter.im/MathewSachin/Screna.svg)](https://gitter.im/MathewSachin/Screna?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
.Net Capture Solution to Capture Screen/Audio/Video/Mouse Cursor/KeyStrokes and more...

Screna provides a highly extensible API to develop Capturing Apps.

# News
* Added an FFMpegVideoWriter in Screna.FFMpeg namespace which uses ffmpeg.exe for encoding.

* Extension packages now supply libraries instead of source-code.

* Merged Screna.Lame into Screna.SharpAvi.

* Added a BASS audio library extension for Screna.

* Screna is now composed of a collection of packages, instead of a single one.  
  Main reasons behind this were licensing issues and the mess involved in copying code from other projects into Screna.
  
Package             | Feature
--------------------|--------------------------------------------
Screna.NAudio       | Audio Recording and Loopback support using [NAudio](https://github.com/NAudio/NAudio) by Mark Heath.
Screna.MouseKeyHook | Mouse Click and Keystroke Overlays using [MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook) by George Mamaladze.
Screna.Bass         | Audio Recording and Loopback support using ManagedBass wrapper over un4seen BASS audio library.
Screna.SharpAvi     | Avi and Lame (Mp3 Encoding) support using [SharpAvi](https://github.com/baSSiLL/SharpAvi) by Vasilli Massilov.


# Getting Started

Install the Package from NuGet.
```powershell
Install-Package Screna
```

> [Captura](https://github.com/MathewSachin/Captura) is a Capture application demonstrating all the features of Screna.
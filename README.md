![Logo](Screna.png)
# Screna
[![Build status](https://ci.appveyor.com/api/projects/status/nadvi6vf6kl999g5/branch/master?svg=true)](https://ci.appveyor.com/project/MathewSachin/screna/branch/master)
![MIT License](https://img.shields.io/github/license/MathewSachin/Screna.svg)
[![Gitter](https://badges.gitter.im/MathewSachin/Screna.svg)](https://gitter.im/MathewSachin/Screna)  
.Net Capture Solution to Capture Screen/Audio/Video/Mouse Cursor/KeyStrokes and more...

Screna provides a highly extensible API to develop Capturing Apps.

# News
* New implementation of Recorder.
* Screna.Bass now includes only a MixedAudioProvider.
* Overlays are now applied using OverlayedImageProvider.
* Added an FFMpegVideoWriter in Screna.FFMpeg namespace which uses ffmpeg.exe for encoding.
* Extension packages now supply libraries instead of source-code.
* Merged Screna.Lame into Screna.SharpAvi.
* Added a BASS audio library extension for Screna.
* Screna is now composed of a collection of packages, instead of a single one.

# Extensions

## Screna.NAudio
Audio Recording and Loopback support using [NAudio](https://github.com/NAudio/NAudio) by Mark Heath.

## Screna.MouseKeyHook
Mouse Click and Keystroke Overlays using [MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook) by George Mamaladze.

## Screna.Bass
Audio Recording and Loopback support using [ManagedBass](https://github.com/ManagedBass/ManagedBass) wrapper over un4seen BASS audio library.

Requires [bass.dll](http://www.un4seen.com/download.php?bass24) and [bassmix.dll](http://www.un4seen.com/download.php?bassmix24).

## Screna.SharpAvi
Avi and Lame (Mp3 Encoding) support using [SharpAvi](https://github.com/baSSiLL/SharpAvi) by Vasilli Massilov.

Requires *lameenc32.dll* or *lameenc64.dll* for Mp3 encoding.
Download from http://lame.sourceforge.net.

### Some Issues playing Avi files on Windows Media Player
- Videos shorter than 9 seconds show error.
- Videos greater than 9 seconds play well but show increased duration.

VLC MediaPlayer was tested with the same files and worked flawlessly.

# Getting Started
> Requires Visual Studio 2017

Install the Package from NuGet.
```powershell
Install-Package Screna
```

> [Captura](https://github.com/MathewSachin/Captura) is a Capture application demonstrating all the features of Screna.

## Support Us
Screna is made and maintained in personal time.
If you feel generous, you could donate to the [Gratipay team](https://gratipay.com/Screna) or [Mathew Sachin on PayPal](https://www.paypal.me/MathewSachin) to show your support.
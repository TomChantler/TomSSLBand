# TomSSLBand
A simple application for the Microsoft Band

This Universal Windows Platform (UWP) App was coded during a live demo earlier today. This means it's not necessarily very robust, doesn't have error handling, etc. It's not production code. But it works<sup>[1](#myfootnote1)</sup>. 

TomSSLBand runs on your Windows 10 device (I used my phone), connects to any Microsoft Bands which are connected and then displays various sensor data on the screen from the first band it found (I was up against the clock and there's usually only one band attached). When you tap the screen to turn data collection on and off you are provided with haptic feedback (in other words it vibrates a bit).

The important things to note are:

* It was easy to code
* It only took about half an hour from start to finish
* It reads live data from your Microsoft Band, which is pretty cool
* The app is still installed on my device, so I can demonstrate it at any time

Full instructions here: https://tomssl.com/2016/02/10/tomssl-band-writing-a-simple-app-for-the-microsoft-band 

**NOTE:** If you're a complete beginner to UWP or Microsoft Band development it's worth reading the blog post and following along. I was a complete beginner to this sort of development a few days ago. The whole thing really won't take you more than half an hour.

---

<a name="myfootnote1">1</a>: At first it didn't work during the demo, mind you. But when I restarted my Microsoft Band it was okay. Perhaps something to bear in mind if you encounter similar issues.
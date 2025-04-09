# Procedural Rhythm Game

A rhythm game where the levels are procedurally generated using audio analysis from a YouTube URL.

## Inspiration

Rhythm games are always very fun, but the number of songs can often be quite limited. Mods tend to solve this problem, but rhythm game modding communities tend to have a fairly specific preferred genre of music and require the manual work of a mapper. If a level can be dynamically created from just a link to a YouTube video, that problem would be solved.
This idea was inspired by Steam game "Beat Aim - Rhythm FPS Trainer" but with gameplay more akin to Osu!.

## What it does
<ul>
<li>Get a Song - Enter the link to any YouTube video you have the rights to the audio of and it will be converted to a Unity AudioClip</li>
<li>Keep the Rhythm - An adjusted version of UniBpmAnalyzer by WestHillApps finds the BPM of the AudioClip to generate targets at</li>
<li>Variation - The AudioClip is searched for areas of high bass, snare, etc. to alter the behaviour of the level. Targets may spawn faster, change size, or change spawn location depending on the audio currently playing </li>
</ul>

## How I built it
This project is built in Unity, using C#.

## What's next for Procedural Rhythm Game
<ul>
<li>More interesting, predefined spawn patterns</li>
<li>More song analysis for unique moments</li>
<li>User difficulty options</li>
</ul>


This project uses UniBpmAnalyzer by WestHillApps (Hironari Nishioka), 
released under the MIT License. 

Copyright (c) 2016 WestHillApps (Hironari Nishioka)

This software is released under the MIT License:
http://opensource.org/licenses/mit-license.php

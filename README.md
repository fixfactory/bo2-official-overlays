# benofficial2's Official Overlays
*Copyright 2023-2026 benofficial2*

## Overview

With benofficial2's Official Overlays you get a complete collection of overlays for [iRacing](https://www.iracing.com/). The overlays are made with [SimHub](https://www.simhubdash.com/download-2/), which means they are fully customizable, and are easy on CPU and memory so you get more FPS in-game. 

Completely free and supported by [donations](https://streamelements.com/benofficial2/tip).

<a href="https://youtu.be/9-xDcU5ww14?si=mpbs5puOl1jgt5Qp&t=901">
<p align="center">
  <img src="Images/Screenshots/Overview-LMP3.png" width="1920"/>
</p>
</a>

> [!NOTE]
> To see them in action, click the image above for a video demo.

### Included in the collection

- **[Inputs Telemetry](#inputs-telemetry)**: shows pedal inputs and a graph
- **[Multi-Class Standings](#multi-class-standings)**: shows the live leaderboard in up to 4 car classes
- **[Relative](#relative)**: shows drivers ahead / behind on track
- **[Spotter](#spotter)**: shows orange bars when side-by-side with another car
- **[Fuel Calculator](#fuel-calculator)**: shows fuel consumption, estimated pit lap and fuel to add.
- **[Track Map](#track-map)**: shows cars on a map
- **[Wind, Track Wetness & Precipitation](#wind-track-wetness--precipitation)**: shows weather-related data fields
- **[Delta Bar](#delta-bar)**: gives you feedback on your pace with time and speed deltas
- **[Highlighted Driver](#highlighted-driver)**: shows information about the currently spectated driver
- **[Dash Overlay](#dash-overlay)**: shows various in-car settings and statuses (DRS, ERS, Fuel, BB, etc.)
- **[Setup Fuel Calculator](#setup-fuel-calculator)**: when in garage, shows how much fuel is needed for the race
- **[Setup Cover](#setup-cover)**: For streamers, hides the setup page when in garage
- **[Twitch Chat](#twitch-chat)**: For streamers, shows your chat on screen
- **[Launch Assist](#launch-assist)**: shows precise pedal inputs to nail a perfect start

## How to Install

[Download benofficial2's Official Overlays](https://github.com/fixfactory/bo2-official-overlays/releases) from the releases page and run the installer. Choose your Sim Hub folder as your installation folder.

> [!TIP]
> See the [Installation Guide](https://github.com/fixfactory/bo2-official-overlays/wiki/Installation-Guide) for a detailed walkthrough on how to install and setup the overlays the first time.

## Inputs Telemetry

This overlay shows the current driver inputs in real-time. A graph with **Pedal Input Traces** shows the recent inputs over time. This is useful to understand bad habits. It is modular meaning you can hide parts you don't need.

<p align="center">
  <img src="Images/Screenshots/Inputs.png" width="500"/>
</p>

## Multi-Class Standings

This overlay shows the live **Leaderboard** for up to 4 car classes. Optional columns: **Car Logo**, **Gap**, **Best Lap Time**, **Last Lap Time**, and **Delta**. See the **benofficial2 plugin** page (in the left menu) for all options.

<p align="center">
  <img src="Images/Screenshots/Standings-IR18.png" width="450"/>
</p>

> [!NOTE]
> The old **Standings** overlay has been deprecated and will not be supported anymore. Please use the **Multi-Class Standings** overlay instead, which can be configured to look the same with only the player's car class.*

## Relative

This overlay shows the nearby drivers **Ahead & Behind** on-track. This includes cars from every car class. In blue are cars that are a lap down and in red are those a lap ahead. The **Out Lap Indicator** shows when a car is on an out-lap. The last column shows the **Gap** in seconds to that car. The header shows the **Strength of Field** and **Incident Count**. The temperature is the **Track Temperature** and the time is your computer's **Local Time**.

<p align="center">
  <img src="Images/Screenshots/Relative.png" width="400"/>
</p>

There's an optional footer (not enabled by default) that shows additional session information typically seen in the Standings header.

## Spotter

This overlay shows orange bars when side-by-side with another car. This complements your audio spotter as it will show up the moment your spotter starts talking. The size of the orange bar gives your an idea of the amount of car overlap and how fast you are passing them so you can time the perfect move. 

<p align="center">
  <img src="Images/Screenshots/Spotter.png" width="400"/>
</p>

Also comes with a **Rejoin Helper** that will show up when stopped or off-track, telling you how safe it is to rejoin by giving you the gap to the next incoming car. Quicker than looking at Relatives.

<p align="center">
  <img src="Images/Screenshots/RejoinHelper.png" width="400"/>
</p>

Designed to be placed over the virtual mirror, but can be placed wherever you want it to be.

## Fuel Calculator

This overlay shows the fuel consumption, indicates when to pit and calculates how much fuel to add. It can also automatically set the fuel to add for you when entering pit lane. See the [documentation](https://github.com/fixfactory/bo2-official-overlays/wiki/Overlay-Documentation#fuel-calculator) for details.

<p align="center">
  <img src="Images/Screenshots/FuelCalc-Box.png" width="400"/>
</p>

## Track Map

This overlay shows car positions on a map. Supports multi-class colors.

<p align="center">
  <img src="Images/Screenshots/Map-Multi.png" width="200"/>
</p>

## Wind, Track Wetness & Precipitation

Three small overlays that show weather-related data fields.

<p align="center">
  <img src="Images/Screenshots/Weather.png" width="150"/>
</p>

## Delta Bar

This overlay gives you feedback on your pace. The **Delta Time** (center) and **Delta Speed** (right) are relative to your session's best clean lap. In qualifying, they are relative to your all-time best lap. On the left, in a race, you get **Gap** and last lap **Interval** information about your nearest competitor ahead and behind. Useful to know if your are improving relative to them.

<p align="center">
  <img src="Images/Screenshots/Delta-Best.png" width="400"/>
</p>

This delta bar can help you improve your lap times by giving you on-the-spot feedback in a corner. Because the **Delta Speed** field is big enough, you'll be able to see it change color while keeping your eyes on the track. So you'll know instantly if your mid-corner speed was faster/slower for example. You'll learn quickly what works and what doesn't.

<p align="center">
  <img src="Images/Screenshots/Delta-White.png" width="400"/>
</p>

> [!NOTE]
> If the delta bar doesn't show immediately, it is because it doesn't have a clean reference lap to compare against yet. It will show up once you complete a full lap without incident.*

<p align="center">
  <img src="Images/Screenshots/Delta-Red.png" width="400"/>
</p>

## Highlighted Driver

This overlay shows information about the currently spectated driver when spectating. Can optionally be shown when in-car as well.

<p align="center">
  <img src="Images/Screenshots/Highlighted-Driver.png" width="400"/>
</p>

## Dash Overlay

This overlay shows various in-car settings and statuses. Useful when the in-game steering wheel is not visible in your FOV. The green boxes typically show statuses about "going faster" such as **DRS**, **ERS**, **P2P**, **OT** (Super Formula), and **Fuel Mix**. 

<p align="center">
  <img src="Images/Screenshots/Dash-ERS-Green.png" width="350"/>
</p>

The yellow boxes are for **Fuel** and **Laps Remaining Estimate**. A **Pit Indicator** will pop up when it's time to pit. The orange boxes are for various in-car settings such as **Entry Diff**. And finally the red boxes are for **Brake Bias** and **ABS** adjustments.

<p align="center">
  <img src="Images/Screenshots/Dash-Pit.png" width="350"/>
</p>

## Setup Fuel Calculator

This overlay shows up only when in the garage / setup screen and tells you how much fuel is needed to start the race. Supports heat races and will determine if the race is limited by time or laps when both are specified. Useful for leagues that have a custom race length.

<p align="center">
  <img src="Images/Screenshots/FuelCalc-2.png" width="350"/>
</p>

A blinking **Fuel Warning** lights up when you're about to grid with an under-fueled setup. Never start a race with a qualification setup again!

<p align="center">
  <img src="Images/Screenshots/Fuel-Warning.png" width="200"/>
</p>

## Setup Cover

For streamers, this overlay shows an animation that hides your car setup values when entering the garage.

## Twitch Chat

For streamers, this overlay shows your chat on-screen so you can keep up with chat. Powered by [ChatIS](https://chatis.is2511.com/).

<p align="center">
  <img src="Images/Screenshots/Twitch-Chat.png" width="500"/>
</p>

## Launch Assist

When stopped, this overlay shows precise pedal input bars to help you consistently hold a desired value. For example with the F1 W13, you set your clutch bitepoint to ~60% and hold 50% throttle. At 52% you might spin out, so this overlay helps you be consistent.

<p align="center">
  <img src="Images/Screenshots/Launch-Assist.png" width="80"/>
</p>

> [!TIP]
> There's some randomness to the clutch bitepoint in iRacing, so make sure to experiment to find a safe value.

## Help & Feedback

For general help with using SimHub, don't hesitate to get help on the [official SimHub Discord server](https://discord.com/invite/nBBMuX7).

For bug reports and feature requests, please [open an issue on Git Hub](https://github.com/fixfactory/bo2-official-overlays/issues).

For help with the overlays, give feedback, and get early access to beta versions, please hop-in my Discord server: [Ben's Official Server](https://discord.gg/s2834nmdYx).

## Credits & Thanks

Thanks to **Wotever** for making [SimHub](https://www.simhubdash.com/download-2/). It is such a powerful and essential piece of sim-racing software. Consider [buying a license](https://www.simhubdash.com/get-a-license/) to support him.

Thanks to **Romainrob** for making the excellent [iRacing Extra Properties](https://www.simhubdash.com/community-2/dashboard-templates/romainrobs-collection/) plugin and his collection of overlays which was a great inspiration.

The overlays in this collection are made by **benofficial2**. 
If you like them, consider following me on [Twitch](https://www.twitch.tv/benofficial2), [YouTube](https://www.youtube.com/@benofficial2?sub_confirmation=1), [Bluesky](https://bsky.app/profile/benofficial2.bsky.social) or making a [donation](https://streamelements.com/benofficial2/tip).

And thank *you* for using them!
# Synthesize
## Introduction
*Synthesize* is a system for interactive procedural audio in Unity.  Its modular approach gives you the ability to generate sound effects and music and alter them at runtime to respond to your game state in ways not possible with fixed sampled audio.
The modular approach used by *Synthesize* is way more flexible than a fixed processing chain synthesizer, allowing you to connect the nodes and route the signals in any way you want.
## Features

## TODO: Unity support
TODO: compatible with Unity mixer and effects system?

## The Node Editor
TODO: What are graphs (=templates). rundown on node editor.
## Running/Hearing a graph
TODO: describe what happens: how graph is converted to runtime stuff.
TODO: MonoAudioGraph setup etc
## Runtime tweakage
TODO: the gameobject hierarchy & modifying vals.

## Audio Synthesis Concepts
Audio synthesis is a large and often complicated subject!  Sometimes the best way to find out what something does is just to play and experiment with it -- that's definitely the case with audio synthesis.
See refs below for further reading!  Here's a quick guide to the basics:
### Waveforms
### Oscillators
Oscillators are the basic building blocks that generate the waveforms to make the audio.
### Synthesis types
TODO: additive, subtractive, FM, others
### Modulation
### Gates & Triggers

### References
- Intro to Computer Music - http://www.indiana.edu/%7eemusic/etext/toc.shtml
- Synthesizer Academy - http://synthesizeracademy.com/
- Book: 'Designing Sound' by Andy Farnell.
- Oscillators - https://www.innovativesynthesis.com/basic-synthesis-part-1-%E2%80%93-oscillators/
- Modulation example app - http://www.iu.edu/~emusic/361/app-modulation.htm
- [Audacity](https://www.audacityteam.org/) - Free open source audio editor.
- TODO: Decomposing sound using spectral analysis
- Procedural audio:
  - https://audioandmusic.wordpress.com/2016/02/19/why-procedural-audio-is-so-useful-for-games-via-asoundeffect-gameaudio-gamedev/  
  - https://www.asoundeffect.com/procedural-game-sound-design/
- DSP coding:
    - http://musicdsp.org/
    - http://www.spinsemi.com/knowledge_base/dsp_basics.html

## Comments/tips/usage/troubleshooting etc
TODO: pops n clicks -- bit about discontinuities, changing vals quickly, smoothing.
TODO: no audio? checklist:
 - Check curves aren't null.
 - some nodes need triggering
 - MonoAudioGraph gameObj setup?  the 'one' sample?
 - spatialised source not near listener?

TODO: notes on DC offset: what/why/caveat!/how fix?
TODO: eula/disclaimer?!

# Reference Section
## Graph Nodes
### Generators
Generators create the basic levels and shapes of signals fed into the audio system. 
#### Wave
The base oscillator to generate standard preset synthesis waveforms.
#### Custom Wave
Like the wave generator, but allows you to specify the waveform explicitly with a curve.
#### Sample
Plays a sampled noise, with frequency and trigger control.
#### Noise
Simple cheap white noise (static) generator.
#### NoiseEx
A more controllable noise generator.
> NoiseEx will probably be your most-used node.  As well as generating noise it can be used as a gate/trigger, random value generator and a useful modulator.
#### LFO
Stands for *Low Frequency Oscillator*.  Very similar to the wave node in effect but usually used at low sub-sonic frequencies as a control/trigger or to modulate a wave's properties rather than create sound itself.
#### Float
A simple constant value used to feed into other modules, or provide a runtime interface point to a graph.

### Effects
Apply to waveforms to change their sound and character fundamentally.
#### Delay
Mixes in a delayed copy of the original signal.
#### Distortion
> Useful for adding 'dirt' to a sound or simulating degraded signal or amplification.
#### Flanger
Applies an animated phasing effect to the sound, modulated with a built-in sine LFO or a user-supplied envelope.  Effectively like applying a frequency-independent comb filter.
> Useful for scifi, laser sounds, rolling explosions.
Note: High feedback can extremely change the sound character!
#### Quantiser
Rounds the signal off to a number of discrete levels.  Effectively a form of distortion.
#### Reverb
Simulates the sound bouncing off the surrounding environment.
### Filters
Filters affect the frequencies present in a waveform.
#### Basic Filter

#### Eq 3-Band
Allows you to alter the amplitude of low, medium and high frequencies present in the signal.
### Util
A collection of utility nodes for envelopes, mathematical operations, mixing/blending, stereo pan and more.
#### ADSR
Changes the amplitude envelope of an incoming signal over time corresponding to a number of control parameters.  A wave's envelope is an important part of its characteristic sound.  In terms of a synth key press:
*Attack*: the time after key is pressed to go from zero to full signal amplitude.
*Decay*: the time from full amplitude to sustain signal level.
*Sustain*: level at which the signal is kept at after the decay period.
*Release*: time taken for the amplitude to fall to zero after key is released.
See: https://www.wikiaudio.org/adsr-envelope/
#### Envelope
Creates an envelope like the ADSR node, but allows more control of the amplitude with a custom curve.
>Tip: to just generate an envelope signal, don't connect the Input to to anything and set it's value to 1.
#### Math
Applies simple mathematical operations to all inputs.
#### MathEx
Allows more specific mathematical operations to be applied to its inputs.
#### Mixer
#### Blender
#### Pan
Splits an input signal into stereo left and right outputs.
#### Response Curve
>Useful to convert an input control signal into a more complex output value.
#### Switcher

## Runtime Integration
Synthesize creates an efficient module structure from a graph's nodes at runtime.
Individual modules can be accessed via the ***MonoAudioGraph FindModule*** methods:
```csharp
ModuleBase FindModule(string aModuleName)
T FindModule<T>(string aModuleName)
T FindModule<T>()
```
Cache these on start and their values can be changed from script to interact with the audio signal at runtime.
Note: inputs that are wired up to another module can not be changed at runtime -- they will have their values overridden by the connected module.
>TIP: It's often useful to create a ***Float*** node in the graph as an easy interface point to one or more modules at runtime.
When used in conjunction with ***Blender*** and ***Response Curve*** nodes this becomes a powerful way of defining and constraining inputs to your graphs.  See the ***TODO*** example for an illustration of this.

## Editor Tools
Some tools are included to view and analyse waveform data.
### Wave Capture
Allows you to capture, view and save the audio output to a WAV file.
### Oscilloscope
Displays the audio waveform in realtime.
TODO: describe osc and modes

## Advanced Usage
### Custom Nodes
Synthesize is built with flexibility and expansion in mind.  You can easily define your own nodes and modules to be used in your graphs.
There are a few conventions to bear in mind when doing so.
TODO: inputs & auto-binding
TODO: name+Val convention & usage?
TODO: Describe tick/getval
TODO: Outputs and OutputValues array.
TODO: implementation of RoutingMode.
TODO: mention poss need for smoothing?
TODO: list util/helper fns?
### Native code
TODO: native code bit

## Glossary
**Dry**
The original unaltered signal upon which e.g. an effect is to be applied.  A Dry/Wet control defines the output mix of input and processed signals.

**Envelope**

**Gate**
A triggering mechanism usually activated by a change in control signal.  A *Gate* is generally associated with the user pressing, holding and releasing a key on synthesizers, usually to trigger an envelope restart.  A Gate is usually considered to be an ongoing control signal rather than a one-off pulse like a trigger.
See also: ***Trigger***.
https://www.noiseengineering.us/blog/2017/4/29/user-questions-triggers-vs-gates

**Modulation**
The changing of a signal (the carrier signal) by combining with another signal in some way.
Amplitude and Frequency Modulation are two major methods used in sound synthesis.  The flanger effect is an example of modulating the time of a delay effect with an LFO.

**Trigger**
A triggering mechanism usually activated by a rising edge/pulse in a control signal.
See also ***Gate***

**Waveform**
The base signal that creates audio, representing the air pressure variation created by speakers that the ear detects.

**Wet**
The fully processed signal output from e.g. an effect.  A Dry/Wet control defines the output mix of input and processed signals.

*[LFO]: Low Frequency Oscillator

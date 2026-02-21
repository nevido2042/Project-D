import wave
import struct
import math
import os

def generate_tone(freq, duration, sample_rate=44100, volume=0.5):
    frames = []
    # simple square wave synthesis for a more retro "chiptune" feel
    for i in range(int(sample_rate * duration)):
        period = int(sample_rate / freq)
        if (i % period) < (period / 2):
            value = int(volume * 32767.0)
        else:
            value = int(-volume * 32767.0)
        frames.append(struct.pack('<h', value))
    return b"".join(frames)

def generate_tetris_bgm(filename):
    notes = [
        (659.25, 0.4), (493.88, 0.2), (523.25, 0.2), (587.33, 0.4), 
        (523.25, 0.2), (493.88, 0.2), (440.00, 0.4), (440.00, 0.2), 
        (523.25, 0.2), (659.25, 0.4), (587.33, 0.2), (523.25, 0.2), 
        (493.88, 0.4), (523.25, 0.2), (587.33, 0.2), (659.25, 0.4), 
        (523.25, 0.4), (440.00, 0.4), (440.00, 0.4)
    ]
    
    with wave.open(filename, 'w') as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(44100)
        
        for _ in range(4): # Loop 4 times
            for freq, dur in notes:
                w.writeframes(generate_tone(freq, dur, volume=0.1))

def generate_sfx(filename, freq, duration, sweep=False):
    with wave.open(filename, 'w') as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(44100)
        frames = []
        volume = 0.2
        sample_rate = 44100
        for i in range(int(sample_rate * duration)):
            current_freq = freq
            if sweep:
                current_freq = freq + (i / sample_rate) * 500  # pitch sweep
            period = int(sample_rate / current_freq)
            if (i % period) < (period / 2):
                value = int(volume * 32767.0)
            else:
                value = int(-volume * 32767.0)
            frames.append(struct.pack('<h', value))
        w.writeframes(b"".join(frames))

os.makedirs("Assets/Audio", exist_ok=True)
generate_tetris_bgm("Assets/Audio/bgm.wav")
generate_sfx("Assets/Audio/move.wav", 400, 0.05)
generate_sfx("Assets/Audio/rotate.wav", 600, 0.05)
generate_sfx("Assets/Audio/lock.wav", 300, 0.1)
generate_sfx("Assets/Audio/clear.wav", 800, 0.2, sweep=True)
generate_sfx("Assets/Audio/gameover.wav", 150, 0.5)

print("Audio generated successfully.")

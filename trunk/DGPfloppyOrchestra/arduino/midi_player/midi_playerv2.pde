//  *****************************************************************************************************************
//  *                                                                                                               *
//  *                                         SpikenzieLabs.com                                                     *
//  *                                                                                                               *
//  *                                   Very Simple Serial to MIDI DEMO                                             *
//  *                                                                                                               *
//  *****************************************************************************************************************
//
// BY: MARK DEMERS 
// May 2009
// VERSION: 0.1
//
// DESCRIPTION:
// Demo sketch to play notes from middle C in the 4th octave up to B in the 5th octave and then back down.
//
//
// HOOK-UP:
// 1. Plug USB cable from Arduino into your computer.
//  
//
// USAGE:
// 1. Install and Set-up Serial MIDI Converter from SpikenzieLabs
// 2. Open, compile, and upload this sketch into your Arduino.
// 3. Run Serial MIDI Converter in the background.
// 4. Launch your music software such as Garage Band or Ableton Live, choose a software instrument and listen to the music.
//
//
// LEGAL:
// This code is provided as is. No guaranties or warranties are given in any form. It is up to you to determine
// this codes suitability for your application.
//

int note = 0;     
int whichByte = 0;
void setup() 
{
  pinMode(13, OUTPUT);
  Serial.begin(57600);                                       // Default speed of the Serial to MIDI Converter serial port
}

void loop() 
{
  digitalWrite(13, HIGH);
  
  MIDI_RX();
  /*
  for(int note=60; note<=83; note++)                        // Going Up
  {
    MIDI_TX(144,note,127);                                  // NOTE ON
    delay(100);

    MIDI_TX(128,note,127);                                  // NOTE OFF
    delay(100);
  }

  for(int note=82; note>=61; note--)                       // Coming Down
  {
    MIDI_TX(144,note,127);                                  // NOTE ON
    delay(100);

    MIDI_TX(128,note,127);                                  // NOTE OFF
    delay(100);
  }
  */
}


void MIDI_TX(unsigned char MESSAGE, unsigned char PITCH, unsigned char VELOCITY) 
{
  Serial.print(MESSAGE);
  Serial.print(PITCH);
  Serial.println(VELOCITY);
}

void MIDI_RX ()
{
  if (Serial.available() > 0) {
    digitalWrite(13, LOW);
    int c = Serial.read();
    
    if (whichByte == 0) // ACTION
    {
      Serial.print("c");
      Serial.print(c & 0x0f);
      Serial.print("a");
      Serial.print(c >> 4);
    }
    else if (whichByte == 1) // Pitch
    {
      Serial.print(" p");
      Serial.print(c);
    }
    else //velocity
    {
      Serial.print(" v");
      Serial.println(c);
    }
    
    whichByte = (whichByte + 1) % 3;
    delay(10);
  }
}



#include <MIDI.h>

int LED = 13;   // select the pin for the LED
int RESET = 7;

int numvalues = 6;

int ANALOG_MESSAGE = 0xE0; // send data for an analog pin (or PWM)
int REPORT_VERSION  = 0xF9; // time since last post
int ENABLE_PIN  = 0xB0; // send message to enable an analog pin 
int DISABLE_PIN = 0xA0; // report firmware version
int values[6];
int newvalues[6];
long lastCheck = millis();
boolean disable_mode;
boolean enable_mode;
int SELECT = 13;
int DIRECTION = 12;
int STEP = 11;

int CHANNELS = 5;

int D4_SELECT = 13;
int D4_DIRECTION = 12;
int D4_STEP = 11;

int D3_SELECT = 10;
int D3_DIRECTION = 9;
int D3_STEP = 8;

int D2_SELECT = 7;
int D2_DIRECTION = 6;
int D2_STEP = 5;

int D1_SELECT = 4;
int D1_DIRECTION = 3;
int D1_STEP = 2;
boolean direction_val = true;
int timer = 0;
int period = 482;
boolean active = true;

struct {
  unsigned char MidiNote;
  unsigned int Timer;
  unsigned int Period;
  unsigned long StealTimer;
  boolean Active;
} 
Generator[4];

int NotePeriod[128] = { 
  12231, 11545, 10897, 10285, 9708, 9163, 8649, 8163, 
  7705, 7273, 6865, 6479, 6116, 5772, 5448, 5143, 4854, 4582, 4324, 4082, 3853, 3636, 3432,
  3240, 3058, 2886, 2724, 2571, 2427, 2291, 2162, 2041, 1926, 1818, 1716, 1620, 1529, 1443,
  1362, 1286, 1213, 1145, 1081, 1020, 963, 909, 858, 810, 764, 722, 681, 643, 607, 573, 541,
  510, 482, 455, 429, 405, 382, 361, 341, 321, 303, 286, 270, 255, 241, 227, 215, 202, 191,
  180, 170, 161, 152, 143, 135, 128, 120, 114, 107, 101, 96, 90, 85, 80, 76, 72, 68, 64, 60,
  57, 54, 51, 48, 45, 43, 40, 38, 36, 34, 32, 30, 28, 27, 25, 24, 23, 21, 20, 19, 18, 17, 16,
  15, 14, 13, 13, 12, 11, 11, 10, 9, 9, 8, 8 };
float BendCoeff[32] = {
  0.99819656, 0.99639637, 0.99459942, 0.99280572, 0.99101525,
  0.98922801, 0.98744400, 0.98566320, 0.98388561, 0.98211123, 0.98034005, 0.97857206,
  0.97680726, 0.97504565, 0.97328721, 0.97153194, 0.96977984, 0.96803090, 0.96628511,
  0.96454247, 0.96280297, 0.96106661, 0.95933338, 0.95760328, 0.95587630, 0.95415243,
  0.95243167, 0.95071402, 0.94899946, 0.94728799, 0.94557961, 0.94387431};


void setup() {
  pinMode(RESET,OUTPUT);   // declare the LED's pin as output

  pinMode(2,OUTPUT);   // step
  pinMode(3,OUTPUT);   // direction
  pinMode(4,OUTPUT);   // select

  pinMode(5,OUTPUT);   // step
  pinMode(6,OUTPUT);   // direction
  pinMode(7,OUTPUT);   // select

  pinMode(8,OUTPUT);   // step
  pinMode(9,OUTPUT);   // direction
  pinMode(10,OUTPUT);   // select  

  pinMode(11,OUTPUT);   // step
  pinMode(12,OUTPUT);   // direction
  pinMode(13,OUTPUT);   // select
  //   digitalWrite(LED, HIGH);
  int i;

  i = 4;
  while(i--)
  {
    Generator[i].Timer = 0;
    Generator[i].StealTimer = 0;
    Generator[i].Active = false;
  }

  MIDI.begin();            	// Launch MIDI with default options
  Serial.end();
  Serial.begin(38400);
  // (input channel is default set to 1)
}



unsigned long time = 0;

void loop () {
  if (MIDI.read()) {     // Is there a MIDI message incoming ?
//    digitalWrite(LED, HIGH);
    ProcessIO();
  }
  interruptCode();
  //testFloppyDrives();
}

void interruptCode(){

  if( Generator[2].Active == true )
  {
    if(micros() >= Generator[2].Timer )
    {
      Generator[2].Timer = micros() + Generator[2].Period * 10;

      digitalWrite(D1_SELECT, LOW);
      delayMicroseconds(8);
      digitalWrite(D1_STEP, LOW);
      delayMicroseconds(8);
      digitalWrite(D1_STEP, HIGH);
      delayMicroseconds(4);
      if(direction_val){
        digitalWrite(D1_DIRECTION, HIGH);
      } 
      else {
        digitalWrite(D1_DIRECTION, LOW);
      }
      direction_val = !direction_val;
    }

  } else { 
    digitalWrite(D1_SELECT, HIGH);
  }

  if( Generator[1].Active == true )
  {
           
          
        
    if(micros() >= Generator[1].Timer )
    {
 
      Generator[1].Timer = micros() + Generator[1].Period * 10;

      digitalWrite(D2_SELECT, LOW);
      delayMicroseconds(8);
      digitalWrite(D2_STEP, LOW);
      delayMicroseconds(8);
      digitalWrite(D2_STEP, HIGH);
      delayMicroseconds(4);
      if(direction_val){
        digitalWrite(D2_DIRECTION, HIGH);
      } else {
        digitalWrite(D2_DIRECTION, LOW);
      }
      direction_val = !direction_val;
    }
  } else {
    digitalWrite(D2_SELECT, HIGH);
  }

//  // drive 3
//  if( Generator[2].Active == true )
//  {
//    if(micros() >= Generator[2].Timer )
//    {
//      Generator[2].Timer = micros() + Generator[2].Period * 10;
//
//      digitalWrite(D3_SELECT, LOW);
//      delayMicroseconds(8);
//      digitalWrite(D3_STEP, LOW);
//      delayMicroseconds(8);
//      digitalWrite(D3_STEP, HIGH);
//      delayMicroseconds(4);
//      if(direction_val){
//        digitalWrite(D3_DIRECTION, HIGH);
//      } else {
//        digitalWrite(D3_DIRECTION, LOW);
//      }
//      direction_val = !direction_val;
//    }
//  } else {
//    digitalWrite(D3_SELECT, HIGH);
//  }

  // drive 4
//  if( Generator[3].Active == true )
//  {
//    if(micros() >= Generator[3].Timer )
//    {
//      Generator[3].Timer = micros() + Generator[3].Period * 10;
//
//      digitalWrite(D4_SELECT, LOW);
//      delayMicroseconds(8);
//      digitalWrite(D4_STEP, LOW);
//      delayMicroseconds(8);
//      digitalWrite(D4_STEP, HIGH);
//      delayMicroseconds(4);
//      if(direction_val){
//        digitalWrite(D4_DIRECTION, HIGH);
//      } else {
//        digitalWrite(D4_DIRECTION, LOW);
//      }
//      direction_val = !direction_val;
//    }
//  } else {
//    digitalWrite(D4_SELECT, HIGH);
//  }
}
/********************************************************************
 * Function:        void ProcessIO(void)
 *
 * PreCondition:    None
 *
 * Input:           None
 *
 * Output:          None
 *
 * Side Effects:    None
 *
 * Overview:        This function is a place holder for other user
 *                  routines. It is a mixture of both USB and
 *                  non-USB tasks.
 *
 * Note:            None
 *******************************************************************/
void ProcessIO()
{

  //INSERT MIDI PROCESSING CODE HERE

  boolean note_stealing = false;
  static byte note;
  int channel;
  byte oldest_channel;
  unsigned long lowest_counter;
  boolean duplicate_note = false;
  float bent_note;
  unsigned char bend_value;
//  digitalWrite(LED, HIGH);  
  switch(MIDI.getType()) {
  
  case NoteOn:
    if(note_stealing)
    {
      
      // Note stealing logic: find the channel with the lowest steal counter,
      // and reassign it to the newest note on command. In the event of a
      // tie, channel order decides.

      oldest_channel = 0;
      lowest_counter = 0xFFFFFFFF;

      note = MIDI.getData1();

      channel = MIDI.getChannel();

      if(Generator[channel].Active == true)
        Generator[channel].StealTimer--;

      if(Generator[channel].MidiNote == note && Generator[channel].Active == true)
        duplicate_note = true;

      if(Generator[channel].StealTimer < lowest_counter)
      {
        lowest_counter = Generator[channel].StealTimer;
        oldest_channel = channel;
      }


      if(!duplicate_note)
      {
        Generator[oldest_channel].MidiNote = note;
        Generator[oldest_channel].Period = NotePeriod[note];
        Generator[channel].Timer = micros() + Generator[channel].Period * 10;
        Generator[oldest_channel].StealTimer = 0xFFFFFFFF;
        Generator[oldest_channel].Active = true;
      }
    }
    else
    {
      
      note = MIDI.getData1();
      channel = MIDI.getChannel();

      if(channel >= 0 && channel < CHANNELS)
      {
        Generator[channel].MidiNote = note;
        Generator[channel].Period = NotePeriod[note];
        Generator[channel].Timer = micros() + Generator[channel].Period*10;
        Generator[channel].Active = true;
      }
    }

    break;

  case NoteOff:
    if(note_stealing)
    {
      channel = MIDI.getChannel();

      note = MIDI.getData1();

      // find the active channel for this note, if it exists
      if(Generator[channel].MidiNote == note)
      {
        //kill it
        Generator[channel].Active = false;
        Generator[channel].StealTimer = 0;
        break;
      }

    }  
    else
    {
      note = MIDI.getData1();
      channel =MIDI.getChannel();

      if(channel >= 0 && channel < CHANNELS)
      {
        if(Generator[channel].MidiNote == note)
        {
          Generator[channel].Active = false;
        }
      }
    }

    break;

    // Please excuse this atrocious bend code. It was rushed to be used, only to not
    // actually be used at all due to a change in song choice. It did work "kind of"
    // when I originally implemented it, but I never ironed the bugs out. In fact it
    // may not work at all in its present state due to changes elsewhere. I definitely
    // did have it working at one point in time for a song demo I decided not to make.

  case PitchBend:
    {
      channel = MIDI.getChannel();


      if(channel >= 0 && channel < CHANNELS)
      {
        note = Generator[channel].MidiNote;
        //bent_note = NotePeriod[note];

        // 100 cents per semitone, PITCH_BEND_RANGE max semitones

        // 1. calculate how many cents to bend pitch ...

        // we can use a piecewise linear interpolation between semitones
        // if we restrict the max bend range to +/- 2 semitones, we can
        // just check if the bend is between 0 and 1 semitones and linearly interpolate,
        // or if it's between 1 and 2 semitones, start at the next highest note, and linearly
        // interpolate to the one above that (or so on for successively higher ranges)

        // 2 ^ 1/12 = 1.05946309435929526456...
        // 2 ^-1/12 = 0.94387431268169349664...

        // for PITCH_BEND_RANGE = 2
        // max cents = PITCH_BEND_RANGE * 100 = 200 cents
        // 0x2000 - 0x3FFF = 200 cents; 0x2000 - 0x0000 = 200 cents

        // 0x2000 = no bend, 0x0000 = full bend down, 0x3FFF = full bend up


        // 1. assemble the 14-bit bend value
        //bend_value = ((int)(Generator[channel].DATA_2 & 0x7F) << 7) | (int)(Generator[channel].DATA_1 & 0x7F);

        // actually, we can probably get away with using only the most significant byte because the floppy-stepper-oscillator is such a crude
        //sound in the first place
        bend_value = MIDI.getData2();



        // 2. determine if the bend is up or down
        if(bend_value > 0x40)
        {
          bend_value -= 0x40; // value between 0x00 and 0x3F

          if(bend_value >= 0x20) // bend past +1 semitone
          {
            bend_value -= 0x20; // value between 0x00 and 0x1F;
            bent_note = NotePeriod[note+1];
          }
          else // bend between 0 and +1 semitone
          {
            bent_note = NotePeriod[note];
          }

          // !! FIX THIS
          // !! we need to bend the cycle period down to bend frequency up, and vice-versa

          //bend_value = 0x20 - bend_value;
          bent_note *= BendCoeff[bend_value];
        }
        else if(bend_value < 0x40)
        {
          bend_value = bend_value - 0x39;
        }

        Generator[channel].Period = bent_note;
        Generator[channel].Timer = micros() + Generator[channel].Period*10;
      }
    }

    break;

  default:

    break;
  }


}//end ProcessIO



void testFloppyDrives(){

  // drive 1
  time = millis() + 1000;
  while(millis() < time){
    if(timer > period) {
      timer = 0;
      digitalWrite(D1_SELECT, LOW);
      delayMicroseconds(8);
      digitalWrite(D1_STEP, LOW);
      delayMicroseconds(8);
      digitalWrite(D1_STEP, HIGH);
      delayMicroseconds(4);
      if(direction_val){
        digitalWrite(D1_DIRECTION, HIGH);
      } 
      else {
        digitalWrite(D1_DIRECTION, LOW);
      }
      direction_val = !direction_val;
    }
    timer ++;
    delayMicroseconds(10);
  }
  digitalWrite(D1_SELECT, HIGH);

  // drive 2
  time = millis() + 1000;
  while(millis() < time){
    if(timer > period) {
      timer = 0;
      digitalWrite(D2_SELECT, LOW);
      delayMicroseconds(8);
      digitalWrite(D2_STEP, LOW);
      delayMicroseconds(8);
      digitalWrite(D2_STEP, HIGH);
      delayMicroseconds(4);
      if(direction_val){
        digitalWrite(D2_DIRECTION, HIGH);
      } 
      else {
        digitalWrite(D2_DIRECTION, LOW);
      }
      direction_val = !direction_val;
    }
    timer ++;
    delayMicroseconds(10);
  }
  digitalWrite(D2_SELECT, HIGH);

  // drive 3
  time = millis() + 1000;
  while(millis() < time){
    if(timer > period) {
      timer = 0;
      digitalWrite(D3_SELECT, LOW);
      delayMicroseconds(8);
      digitalWrite(D3_STEP, LOW);
      delayMicroseconds(8);
      digitalWrite(D3_STEP, HIGH);
      delayMicroseconds(4);
      if(direction_val){
        digitalWrite(D3_DIRECTION, HIGH);
      } 
      else {
        digitalWrite(D3_DIRECTION, LOW);
      }
      direction_val = !direction_val;
    }
    timer ++;
    delayMicroseconds(10);
  }
  digitalWrite(D3_SELECT, HIGH);

  // drive 4
  time = millis() + 1000;
  while(millis() < time){
    if(timer > period) {
      timer = 0;
      digitalWrite(D4_SELECT, LOW);
      delayMicroseconds(8);
      digitalWrite(D4_STEP, LOW);
      delayMicroseconds(8);
      digitalWrite(D4_STEP, HIGH);
      delayMicroseconds(4);
      if(direction_val){
        digitalWrite(D4_DIRECTION, HIGH);
      } 
      else {
        digitalWrite(D4_DIRECTION, LOW);
      }
      direction_val = !direction_val;
    }
    timer ++;
    delayMicroseconds(10);
  }
  digitalWrite(D4_SELECT, HIGH);  
} // end testFloppyDrives()





void BlinkLed(byte num) { 	// Basic blink function
  for (byte i=0;i<num;i++) {
    digitalWrite(LED,HIGH);
    delay(50);
    digitalWrite(LED,LOW);
    delay(50);
  }
}



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
int SELECT = 7;
int DIRECTION = 6;
int STEP = 5;
int D2_SELECT = 4;
int D2_DIRECTION = 3;
int D2_STEP = 2;
boolean direction_val = true;
int timer = 0;
int period = 45;
boolean active = true;
int NotePeriod[128] = { 12231, 11545, 10897, 10285, 9708, 9163, 8649, 8163, 
7705, 7273, 6865, 6479, 6116, 5772, 5448, 5143, 4854, 4582, 4324, 4082, 3853, 3636, 3432,
3240, 3058, 2886, 2724, 2571, 2427, 2291, 2162, 2041, 1926, 1818, 1716, 1620, 1529, 1443,
1362, 1286, 1213, 1145, 1081, 1020, 963, 909, 858, 810, 764, 722, 681, 643, 607, 573, 541,
510, 482, 455, 429, 405, 382, 361, 341, 321, 303, 286, 270, 255, 241, 227, 215, 202, 191,
180, 170, 161, 152, 143, 135, 128, 120, 114, 107, 101, 96, 90, 85, 80, 76, 72, 68, 64, 60,
57, 54, 51, 48, 45, 43, 40, 38, 36, 34, 32, 30, 28, 27, 25, 24, 23, 21, 20, 19, 18, 17, 16,
15, 14, 13, 13, 12, 11, 11, 10, 9, 9, 8, 8 };
float BendCoeff[32] = {0.99819656, 0.99639637, 0.99459942, 0.99280572, 0.99101525,
0.98922801, 0.98744400, 0.98566320, 0.98388561, 0.98211123, 0.98034005, 0.97857206,
0.97680726, 0.97504565, 0.97328721, 0.97153194, 0.96977984, 0.96803090, 0.96628511,
0.96454247, 0.96280297, 0.96106661, 0.95933338, 0.95760328, 0.95587630, 0.95415243,
0.95243167, 0.95071402, 0.94899946, 0.94728799, 0.94557961, 0.94387431};


void setup() {
  pinMode(LED,OUTPUT);   // declare the LED's pin as output
  pinMode(RESET,OUTPUT);   // declare the LED's pin as output
 
  pinMode(5,OUTPUT);   // step
  pinMode(6,OUTPUT);   // direction
  pinMode(7,OUTPUT);   // select
   digitalWrite(LED, HIGH);
}



void loop () {
  if(timer > period) {
    timer = 0;
    digitalWrite(SELECT, LOW);
    delayMicroseconds(8);
    digitalWrite(STEP, LOW);
    delayMicroseconds(8);
    digitalWrite(STEP, HIGH);
    delayMicroseconds(4);
    if(direction_val){
    digitalWrite(DIRECTION, HIGH);
    } else {
    digitalWrite(DIRECTION, LOW);
    }
    direction_val = !direction_val;
  }
  timer ++;
  delayMicroseconds(10);
  
  
}


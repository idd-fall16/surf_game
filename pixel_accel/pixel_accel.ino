#include "neopixel.h"
#include "Adafruit_LIS3DH.h"
#include "Adafruit_Sensor.h"


#if defined(ARDUINO) 
SYSTEM_MODE(SEMI_AUTOMATIC); 
#endif
 
#define PIN_CENTER      D3
#define N_CENTER 32
#define PIN_SIDE0      D4
#define N_SIDE0 14
#define PIN_SIDE1      D2
#define N_SIDE1 14
#define PIXEL_TYPE WS2812B
#define SAMPLES 5
#define STRIP0 0
#define STRIP1 20
#define STRIP2 40
#define PITCH_MAX 45
#define ROLL_MAX 45
#define BUTTON D8

#define PI 3.1415

typedef struct ACCEL_DATA {
  float xG, yG, zG;
  float pitch, roll;
};

//initialize neopixel lights
Adafruit_NeoPixel strip_center = Adafruit_NeoPixel(N_CENTER, PIN_CENTER, PIXEL_TYPE);
Adafruit_NeoPixel strip_side0 = Adafruit_NeoPixel(N_SIDE0, PIN_SIDE0, PIXEL_TYPE);
Adafruit_NeoPixel strip_side1 = Adafruit_NeoPixel(N_SIDE1, PIN_SIDE1, PIXEL_TYPE);


// Accelerometer 
ACCEL_DATA accel_data;
float pitch_data[SAMPLES] = {0};
float roll_data[SAMPLES] = {0};
float average_roll, average_pitch, total_roll, total_pitch;
Adafruit_LIS3DH lis = Adafruit_LIS3DH();

//get color step for smooth transition
int color_step_center = 255/N_CENTER;
int color_step_side = 255/N_SIDE0;
int sample_count = 0;
 
void setup() {
  WiFi.off(); //turn off Wifi

  //begin LEDs
  strip_center.begin();
  strip_side0.begin();
  strip_side1.begin();

  //start acceleromtere
  LIS3DH_init();

}
 
void loop() {
  //Get accelerometer data
   LIS3DH_getData();

  //send accelerometer data
   //LIS3DH_sendData();

   //apply avergage filter
    if(sample_count > SAMPLES)
      sample_count = 0;
    pitch_data[sample_count] = accel_data.pitch;
    roll_data[sample_count] = accel_data.roll;
    sample_count++;

    for(int average_count = 0; average_count < SAMPLES; average_count++){
      total_pitch += pitch_data[average_count];
      total_roll += roll_data[average_count];
    }

  average_pitch = total_pitch/SAMPLES;
  average_roll = total_roll/SAMPLES;

  total_pitch = 0;
  total_roll = 0;

  //update lights based on accelerometer data
  tilt_light();
}

void tilt_light(void){
  
  //map roll to a pixel value for center strip
  int light_pitch_center = map(average_pitch,-PITCH_MAX,PITCH_MAX,0,N_CENTER);
  //map roll to a pixel value for side strip
  int light_pitch_side = map(average_pitch,-PITCH_MAX,PITCH_MAX,0,N_SIDE0);

  //map light pitch RGB value
  int light_roll = map(abs(average_roll),0,ROLL_MAX,0,255);

  //set color for center strip
    for(uint16_t i=0; i<N_CENTER; i++) {
      //set each pixel
      strip_center.setPixelColor(i,strip_center.Color((round(color_step_center*abs(light_pitch_center-i))),(round(color_step_center*abs(light_pitch_center-i))),(255)));
      //show lights  
      strip_center.show();
     }
     
    //set color for side strip
    for(uint16_t i=0; i<N_SIDE0; i++) {

      //Determine polarity of pitch angle
      if(average_pitch > 0){
        //set each pixel
         strip_side0.setPixelColor(i,strip_side0.Color(round(color_step_center*abs(light_pitch_side-i)),light_roll,255));
         strip_side1.setPixelColor(i,strip_side1.Color(light_roll,round(color_step_center*abs(light_pitch_side-i)),255));
      }
      else if(average_pitch < 0) {
          //set each pixel
          strip_side0.setPixelColor(i,strip_side0.Color(light_roll,round(color_step_center*abs(light_pitch_side-i)),255));
          strip_side1.setPixelColor(i,strip_side1.Color(round(color_step_center*abs(light_pitch_side-i)),light_roll,255));
      }
      else {
          //set each pixel
          strip_side0.setPixelColor(i,strip_side0.Color(round(color_step_center*abs(light_pitch_side-i)),round(color_step_center*abs(light_pitch_side-i)),255));
          strip_side1.setPixelColor(i,strip_side1.Color(round(color_step_center*abs(light_pitch_side-i)),round(color_step_center*abs(light_pitch_side-i)),255));
      }

      //show lights
      strip_side0.show();
      strip_side1.show();

  }

  
}

boolean LIS3DH_init(void) {
        if(!lis.begin(0x18)) {
                Serial.printf("Couldn't start!\n");
                return 0;
        }
        Serial.printf("LIS3DH Found");

        lis.setRange(LIS3DH_RANGE_4_G);

        Serial.printf("Range = %d G \n",lis.getRange());
        return 1;
}


void LIS3DH_sendData(void) {
        Serial.printf("%f,%f,%i\n",accel_data.pitch,accel_data.roll,!digitalRead(BUTTON));
}


void LIS3DH_getData(void) {
        //REMINDER, I2C Pins SDA1==D0, SCL1 == D1
        // get X Y and Z data at once
        lis.read();
        //get a new sensor event, normalized
        sensors_event_t event;
        lis.getEvent(&event);
        //Get the results (acceleration measured in m/s^2)
        accel_data.xG = (event.acceleration.x/(9.8));
        accel_data.yG = (event.acceleration.y/(9.8));
        accel_data.zG = (event.acceleration.z/(9.8));

        accel_data.pitch = atan(accel_data.xG/sqrt(pow(accel_data.yG,2) + pow(accel_data.zG,2)));
        accel_data.roll = atan(accel_data.yG/sqrt(pow(accel_data.xG,2) + pow(accel_data.zG,2)));

        accel_data.pitch = accel_data.pitch * (180/PI);
        accel_data.roll = accel_data.roll * (180/PI);
}

static inline int8_t sign(int val) {
 if (val < 0) return -1;
 if (val==0) return 0;
 return 1;
}

int8_t limit(int val) {
  if(val > 255) return 255;
  if(val < 0) return 0;
  return val;
}



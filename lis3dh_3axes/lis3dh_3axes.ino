/* Graph I2C Accelerometer On RedBear Duo over Serial Port
 * Adafruit Part 2809 LIS3DH - http://adafru.it/2809
 * This example shows how to program I2C manually
 * I2C Pins SDA1==D0, SCL1 == D1
 * Default address: 0x18
 */
 
// do not use the cloud functions - assume programming through Arduino IDE
#if defined(ARDUINO) 
SYSTEM_MODE(SEMI_AUTOMATIC); //SYSTEM_MODE(MANUAL); 
#endif
// Basic demo for accelerometer readings from Adafruit LIS3DH

// your network name also called SSID
char ssid[] = "juanito";
// your network password
char password[] = "holahola";

uint16_t port = 5555;     // port number of the server
IPAddress server(192,168,2,2);//169,229,123,19);//192,168,2,5);//74,125,115,105);   // IP Address of the server
TCPClient client;

void printWifiStatus();

#include "Adafruit_LIS3DH.h"
#include "Adafruit_Sensor.h"

#define PI 3.1415
#define button D8

typedef struct ACCEL_DATA {
  float xG, yG, zG;
  float pitch, roll;
};

// Accelerometer 
ACCEL_DATA accel_data;
// I2C
Adafruit_LIS3DH lis = Adafruit_LIS3DH();

void setup(void) {
  
  Serial.begin(9600);
  pinMode(button,INPUT_PULLUP);
  
  LIS3DH_init();

  // attempt to connect to Wifi network:
  Serial.print("Attempting to connect to Network named: ");
  // print the network name (SSID);
  Serial.println(ssid); 
  // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
  WiFi.on();
  WiFi.setCredentials(ssid, password);
  WiFi.connect();

  while (WiFi.connecting()) {
    // print dots while we wait to connect
    Serial.print(".");
    delay(300);
  }
  
  Serial.println("\nYou're connected to the network");
  Serial.println("Waiting for an ip address");
  
  IPAddress localIP = WiFi.localIP();
  while (localIP[0] == 0) {
    localIP = WiFi.localIP();
    Serial.println("waiting for an IP address");
    delay(1000);
  }

  Serial.println("\nIP Address obtained");
  
  // you're connected now, so print out the status  
  printWifiStatus();
  
  // attempt to connect to the server
  Serial.println("Attempting to connect to server");

  uint8_t tries = 0;
  while (client.connect(server, port) == false) {
    Serial.print(".");
    if (tries++ > 100) {
      Serial.println("\nThe server isn't responding");
      while(1);
    }
    delay(100);
  }
  
  //we've connected to the server by this point
  Serial.println("\nConnected to the server!");
  
}

void loop() {
  LIS3DH_getData();
  //LIS3DH_sendData();
  delay(33); //30Hz update rate

  if (Serial.available()) {
    //read the serial command into a buffer
    char buffer[255] = {0};
    Serial.readBytes(buffer, Serial.available());
    //send the serial command to the server
    client.println(buffer);
    Serial.print("Sent: ");
    Serial.println(buffer);
    //client.printf("%f,%f",accel_data.pitch,accel_data.roll);
  }
  
  if (client.available()) {
    char buffer[255] = {0};
    client.read((uint8_t*)buffer, client.available());
    Serial.print("Received: ");
    Serial.println(buffer);
    //client.printf("%f,%f",accel_data.pitch,accel_data.roll);
  }  
  client.printf("%f,%f,%i\n",accel_data.pitch,accel_data.roll,!digitalRead(button));
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
        Serial.printf("%f,%f,%i\n",accel_data.pitch,accel_data.roll,!digitalRead(button));
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

void printWifiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("Network Name: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
}

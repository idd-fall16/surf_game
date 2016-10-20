
#if defined(ARDUINO) 
SYSTEM_MODE(SEMI_AUTOMATIC); 
#endif
 

void setup() {
  WiFi.off(); //turn off Wifi
  Serial.begin (9600);  

 
}
 
void loop() {

  Serial.printf("1.0,0\n");
  delay(500);
  Serial.printf("0,-1.0\n");
  delay(500);
  Serial.printf("-1.0,0\n");
  delay(500);
  Serial.printf("0,1.0\n");
  delay(500);  
}



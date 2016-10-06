/**
 * Circle Collision with Swapping Velocities
 * by Ira Greenberg. 
 * 
 * Based on Keith Peter's Solution in
 * Foundation Actionscript Animation: Making Things Move!
 */
import processing.sound.*;
import processing.net.*;
import processing.serial.*;  
boolean data_mode = true; //flase for serial, true for wifi
// import the serial lin
Serial myPort;                               // the serial port
//variables from serial port (coming from RedBear Duo)
float pitch=0, roll=0; 
int feetsensor=0;  
Ball board = new Ball(50, 50, 10);
float pitchspeed=0.3;
float rollspeed=0.3;

//board position variables
int count_target_update, frame_targe_transition = 10;
PVector vector_target, vector_player, vector_delta, vector_target_draw;
float target_pitch, target_roll;
float new_target_pitch, new_target_roll;
boolean new_target = true;
boolean holding = false;
boolean nailed_it = false;
//state variables
int state = 0;
boolean moving_roll = true, moving_pitch = true;
String[] nailed_it_strings = {"Nailed It!", "Radical!", "Get Pitted!", "Gnarly!", "Killing It!"};
int num_string = 5;
int i = 0;
boolean printed_text = false;
float eps = 20;
float tintr = 255, tintb = 255, tintg = 255;
//font variables
PFont font_game;
//time control variables
int time_game;
int time_level=0;
int hold = 500;
int hold_time, time;
int time_to_fail=20;
int time_to_fail_decrease=0;
//sound declaration
SoundFile ding, surf_music;
SoundFile wack, so_pitted, bro, whoo, so_sick;
int sound_length = 4;
int sound = 0;

//score
int score=0;

//average
int angle_count = 0;
int samples = 5;
float[] pitch_array = new float[10];
float[] roll_array = new float[10];
float total_roll = 0;
float total_pitch = 0;

int flagcollision = 0;
float collisiontime;
 
Ball[] balls =  { 
  new Ball(100, 400, 30), 
  new Ball(700, 400, 100) 
  //new Ball(500, 100, 40) 
};

//wifi set up
int port = 5555; //Sever TCP-Port number
Server server;
String strdata = "0";

void setup() {
  frameRate(50);
  size(736, 552);//640, 360);
  if(data_mode){
    server = new Server(this, port);
    println(server.ip());
    noStroke(); 
  }  
  
  if(!data_mode){
    myPort = new Serial(this, Serial.list()[0], 9600);
    // only generate a serial event when you get a newline: 
    myPort.bufferUntil('\n');
  }
  board.position.set(width/2,height/2);
  board.velocity.set(0,0);
  font_game = createFont("zorque", 100, true);
}

void draw() {
  
  if(data_mode){
    Client client = server.available(); // 
    /* Communication with client */
    if(client != null){      // if client is  connecting
        client.write("\n");
        if((strdata=client.readString()) != null){ 
            strdata=trim(strdata);
            //println(strdata);
        }
      strdata = trim(strdata);
      String items[] = split(strdata, ','); 
      if(items.length==3) {
        pitch = float(items[0]);
        roll = float(items[1]);
        feetsensor = int(items[2]); }     
      //vector_player.set(pitch,roll);  
    }  
  }  
  
  background(51);

  for (Ball b : balls) {
    b.update();
    b.display();
    b.checkBoundaryCollision();
  }
  //user board drawing
  //board.position.set(-pitch+width/2,roll+height/2);
  board.position.add(-pitch*pitchspeed,roll*rollspeed);
  board.velocity.set(-pitch*pitchspeed,roll*rollspeed);
  board.checkBoundaryCollision();
  board.display();
  
  //check collisions btw balls
  balls[0].checkCollision(balls[1]);
  
  //check collision btw user and balls
  if(flagcollision==0) {   
    for (Ball b : balls) {
      flagcollision = b.checksingleCollision(board);
      if(flagcollision==1) {
        collisiontime = millis();
        time_level = -(int(millis()/1000));
        break;
      }    
    } 
  }
  
  //this gives some time to run away without reseting timer
  if(flagcollision==1) {
    if((millis()-collisiontime)>1000){
      flagcollision=0;
    }
  }  
  //print pitch and roll to check if serial port is working well
  println(pitch,roll,feetsensor);
  println(flagcollision);
  if (time_game>10){
    time_level = -(int(millis()/1000));
    score=score+1;
  }
  
  
  drawtimer();
  drawscore();  
}


void serialEvent(Serial myPort) {
  // read the serial buffer:
  String myString = myPort.readStringUntil('\n');
  // if you got any bytes other than the linefeed: 
  if (myString != null) {
    myString = trim(myString);
    String items[] = split(myString, ','); 
    pitch = float(items[0]);
    roll = float(items[1]);
    feetsensor = int(items[2]);
  }
}

void drawtimer(){
  textFont(font_game, 36);
  String time;
  time_game = time_level+(int(millis()/1000));
  time = nf(time_game , 2); 
  text("Timer: ", width*8/10, height*1/10);  
  text(time, width*8/10, height*1.7/10);  
}

void drawscore(){
  textFont(font_game, 36);
  text("Score: ", width*0.5/10, height*1/10);  
  text(score, width*0.5/10, height*1.7/10);  
}
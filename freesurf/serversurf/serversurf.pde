/*************************************/
/* Arduino meets Processing via Wifi */
/*   Processing Sever side sketch    */
/*************************************/

import processing.net.*;

int port = 5555; //Sever TCP-Port number

Server server;
String strdata = "0";

void setup(){
  size(400,400,P3D);
  server = new Server(this, port);
  println(server.ip());
  noStroke(); 

}

void draw(){
  Client client = server.available(); // 
  /* Communication with client */
  if(client != null){      // if client is  connecting
      client.write("\n");
      if((strdata=client.readString()) != null){ 
          strdata=trim(strdata);
          println(strdata);
      }
  }
  
}

/* When a client is connected */
void serverEvent(Server s, Client c){
  println("Client connect - IP:"+c.ip()); // show IP 
}
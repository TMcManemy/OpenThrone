#include <NewPing.h>
#include <SPI.h>
#include "RF24.h"

#define TRIGGER_PIN 9
#define ECHO_PIN 8
#define MAX_DISTANCE 400 //centimeters
#define LOOP_DELAY 2000
#define REFRESH_FREQUENCY 20 //seconds
#define TOLERANCE 5 //centimeters
#define THIS_NODE 1 //stall number (0, 1, or 2)

NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
RF24 myRadio (6,7);

byte addresses[][6] = {"1Node", "2Node", "3Node"};
int referenceDistance;
unsigned long lastCalledHome = 0;
bool occupied = false;

void setup() {
  referenceDistance = learnDistance();
  
  myRadio.begin();
  myRadio.setChannel(108);
  myRadio.setPALevel(RF24_PA_MIN);
  //  myRadio.setPALevel(RF24_PA_MAX);  // Uncomment for more power
  myRadio.openWritingPipe(addresses[THIS_NODE]);
  delay(1000);
}

void loop() {
  int currentDistance = 0;
  while (currentDistance == 0) {
    currentDistance = sonar.ping() / US_ROUNDTRIP_CM;
  }
  if (currentDistance > (referenceDistance + TOLERANCE) || currentDistance < (referenceDistance - TOLERANCE)) {
    if (!occupied) {
      occupied = true;
      myRadio.write( &occupied, sizeof(bool) );
      lastCalledHome = millis();
    }
  } else {
    if (occupied) {
      occupied = false;
      myRadio.write( &occupied, sizeof(bool) );
      lastCalledHome = millis();
    }
  }

  if (lastCalledHome + (REFRESH_FREQUENCY * 1000) < millis()) {
    myRadio.write( &occupied, sizeof(bool) );
    lastCalledHome = millis();
  }
  
  delay(LOOP_DELAY);
}

int learnDistance() {
  int pings = 100;
  int totalTime = 0;
  for (int i = 0; i < pings; i++) {
    int distanceCm = sonar.ping()/US_ROUNDTRIP_CM;
    totalTime += distanceCm;
    if (distanceCm == 0) i--;
    delay(50);
  }
  return totalTime/pings;
}

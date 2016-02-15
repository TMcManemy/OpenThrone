#include <NewPing.h>
#include <SPI.h>
#include "RF24.h"

#define TRIGGER_PIN
#define ECHO_PIN
#define MAX_DISTANCE 400 // in centimeters

NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
RF24 myRadio (6,7);

byte addresses[][6] = {"1Node"};
int currentDistance;

void setup() {
  myRadio.begin();
  myRadio.setChannel(108);
  myRadio.setPALevel(RF24_PA_MIN);
  //  myRadio.setPALevel(RF24_PA_MAX);  // Uncomment for more power
  myRadio.openWritingPipe( addresses[0]);
  delay(1000);
}

void loop() {
  unsigned int uS = sonar.ping(); // Send ping, get ping time in microseconds (uS).
  currentDistance = uS / US_ROUNDTRIP_CM; //convert to cm
  myRadio.write( &currentDistance, sizeof(currentDistance) );
  delay(300);
}

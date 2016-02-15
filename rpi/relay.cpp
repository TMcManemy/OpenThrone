#include <cstdlib>
#include <iostream>
#include <sstream>
#include <string>
#include <unistd.h>
#include <RF24/RF24.h>

using namespace std;

RF24 radio(22,0);
// Assign a unique identifier for this node, 0 or 1
bool radioNumber = 0;

// Radio pipe addresses for the 2 nodes to communicate.
const uint8_t pipes[][6] = {"1Node","2Node"};


int main(int argc, char** argv){
    radio.begin();
    radio.setChannel(108);
    radio.openReadingPipe(1,pipes[0]);
    radio.startListening();

	while (1)
	{
	    int started_waiting_at = millis();
        bool timeout = false;
        while ( ! radio.available() && ! timeout ) {
            if (millis() - started_waiting_at > 200 )
                timeout = true;
        }

        if ( timeout )
        {
            printf("Failed, response timed out.\n");
        }
        else
        {
            int data;
            radio.read( &data, sizeof(int) );
            printf("Got response %i\n", data);
        }
        sleep(1);
	}

  return 0;
}


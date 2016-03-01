#include <cstdlib>
#include <iostream>
#include <sstream>
#include <string>
#include <unistd.h>
#include <RF24/RF24.h>
#include "restclient-cpp/restclient.h"

using namespace std;

RF24 radio(22,0);

const uint8_t pipes[][6] = {"1Node","2Node","3Node"};

int main(int argc, char** argv){
    radio.begin();
    radio.setChannel(108);
    radio.openReadingPipe(1,pipes[0]);
    radio.openReadingPipe(2,pipes[1]);
    radio.openReadingPipe(3,pipes[2]);
    radio.startListening();

	while (1)
	{
	    uint8_t stallNum;
        while (!radio.available(&stallNum)) {
            sleep(1);
        }
        bool occupied;

        radio.read( &occupied, sizeof(bool) );
        printf("Got response %s from stall %i \n", occupied ? "occupied" : "empty", stallNum);

        ostringstream json, address;
        json << "{\"available\": \"" << !occupied << "\"}";
        address << "http://openthrone.com/api/stall/" << (int)stallNum;
        RestClient::Response r = RestClient::put(address.str(), "text/json", json.str());
        cout << r.code << "\n";
	}

  return 0;
}


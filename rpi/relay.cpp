#include <cstdlib>
#include <iostream>
#include <sstream>
#include <string>
#include <unistd.h>
#include <RF24/RF24.h>
#include "restclient-cpp/restclient.h"
#include "restclient-cpp/connection.h"

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
	
	RestClient::init();
	RestClient::Connection conn = RestClient::Connection("http://openthrone.com/api/stall");
	conn.AppendHeader("Authorization", "DontHackMeBro");
	conn.AppendHeader("Content-Type", "application/json");
	
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
		address << "/" << (int)stallNum;
		RestClient::Response r = conn.put(address.str(), json.str());
		cout << r.code << "\n";
	}

	return 0;
}


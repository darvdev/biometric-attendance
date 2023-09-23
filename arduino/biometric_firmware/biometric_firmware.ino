#include <Adafruit_Fingerprint.h>

SoftwareSerial serial(2, 3);
Adafruit_Fingerprint sensor = Adafruit_Fingerprint(&serial);

unsigned long previousMs = 0;
String string = "";
int status = -1;
uint8_t id = 0;
bool fingerState = false;

void setup() {

  delay(10);
  
  pinMode(13, OUTPUT);
  digitalWrite(13, 0);

  Serial.begin(9600);
  delay(10);

  sensor.begin(57600);
  delay(10);
  
  sensor.LEDcontrol(false);
  delay(10);

  if (sensor.verifyPassword()) {

    sensor.getParameters();
    sensor.getTemplateCount();

    if (sensor.templateCount == 0) {
      status = 301; //No fingerprint registered
      digitalWrite(13, 1);

    } else {
      status = 0; //Standby
      digitalWrite(13, 1);

    }

  } else {
    status = 300; //Sensor not connected/detected

  }

}

void loop() {
  unsigned long ms = millis();

  if (ms - previousMs >= 50)  {
    previousMs = ms;
    
    if (status == 100) {
      startFingerprintAttendance();
    } else if (status == 200) {
      if (id > 0) {
        enrollFingerprintStep1();
      }
    } else if (status == 201) {
      if (id > 0) {
        enrollFingerprintStep2();
      }
    } else if (status == 203) {
      if (id > 0) {
        enrollFingerprintStep3();
      }
    }

  }

}


void serialEvent() {
  while (Serial.available() > 0) {
    char read = (char)Serial.read();

    string.toLowerCase();
    
    if (read == '\n') {

      if (string == "connect") {
        Serial.println("ok");
        
      } else if (string == "start") {
        sensor.LEDcontrol(true);
        status = 100; //Start reading fingerprint for attendance
        Serial.println(F("start"));

      } else if (string == "status") {
        Serial.print(F("status="));Serial.println(status);

      } else if (string == "enroll") {
        status = 200; //Start reading fingerprint for enrollment
        sensor.LEDcontrol(false);

      } else if (string == "standby") {
        status = 0; //Stop sensor operation
        sensor.LEDcontrol(false);
        Serial.println(F("standby"));

      } else {

        if(status == 200 && string.indexOf("id=") > -1) {
          string.replace("id=", "");
          id = string.toInt();
          if (id > 0) {
            sensor.LEDcontrol(true);
            Serial.println(F("enroll"));
            
          }

        } else {
          Serial.print(F("$msg|error=Unknown command \""));Serial.print(string);Serial.println(F("\""));

        }

      }
    
      string = "";
    } else {
      string += read;

    }

  }
}


void startFingerprintAttendance() {

  uint8_t p = sensor.getImage();
  if (p != FINGERPRINT_OK) {
    if (p != FINGERPRINT_NOFINGER) {
      //FINGERPRINT_NOFINGER              0x02    - No fingerprint detected
      //FINGERPRINT_PACKETRECIEVEERR      0x01    - Communication error
      //FINGERPRINT_IMAGEFAIL             0x03    - Imaging error
      //Unknown error
      Serial.print(F("$start|code="));Serial.println(p);
    }
    return;
  }

  //FINGERPRINT_OK                        0x00    - OK
  Serial.println(F("$start|scan"));
  delay(1500);

  p = sensor.image2Tz();
  if (p != FINGERPRINT_OK) {
    //FINGERPRINT_IMAGEMESS           0x06    - Image too messy
    //FINGERPRINT_PACKETRECIEVEERR    0x01    - Communication error
    //FINGERPRINT_FEATUREFAIL         0x07    - Could not find fingerprint features
    //FINGERPRINT_INVALIDIMAGE        0x15    - Could not find fingerprint features
    Serial.print(F("$start|code="));Serial.println(p);
    return;
  }

    //FINGERPRINT_OK                  0x00    - OK

  p = sensor.fingerSearch();
  if (p != FINGERPRINT_OK) {
    //FINGERPRINT_PACKETRECIEVEERR    0x01
    //FINGERPRINT_NOTFOUND            0x09
    //Unknown error
    Serial.print(F("$start|code="));Serial.println(p);
    return;
  }

  //FINGERPRINT_OK                    0x00  Found a print match

  Serial.print(F("$start|id="));Serial.println(sensor.fingerID);//Serial.print(F(",confidence="));Serial.println(sensor.confidence); //Send sensor data to app
  delay(2000);
}

void enrollFingerprintStep1() {

  int p = sensor.getImage();
  if (p != FINGERPRINT_OK) {
    if (p != FINGERPRINT_NOFINGER) {
      //Error
      //FINGERPRINT_PACKETRECIEVEERR        0x01
      //FINGERPRINT_IMAGEFAIL               0x03
      //Unknown error
      Serial.print(F("$enroll|code="));Serial.println(p);
    }
    return;
  }

  //FINGERPRINT_OK                          0x00
  fingerState = true;

  p = sensor.image2Tz(1);
  if (p != FINGERPRINT_OK) {
    //FINGERPRINT_IMAGEMESS                 0x06
    //FINGERPRINT_PACKETRECIEVEERR          0x01
    //FINGERPRINT_FEATUREFAIL               0x07
    //FINGERPRINT_INVALIDIMAGE              0x15
    //Unknown error
    Serial.print(F("$enroll|code="));Serial.println(p);
    return;
  }

  Serial.println(F("$enroll|ok"));//Send enroll step 1 success
  delay(2000);
  status = 201; //Enroll fingerprint go to step 2
}

void enrollFingerprintStep2() {
  int p = sensor.getImage();

  //Removed finger in sensor to go to step 3
  if (p == FINGERPRINT_NOFINGER) {
    status = 203;
    Serial.println(F("$enroll|next"));//Send enroll step 2 success
  }

}


void enrollFingerprintStep3() {

  int p = sensor.getImage();
  if (p != FINGERPRINT_OK) {
    if (p != FINGERPRINT_NOFINGER) {
      //FINGERPRINT_PACKETRECIEVEERR    0x01
      //FINGERPRINT_IMAGEFAIL           0x03
      //Unknown error
      Serial.print(F("$enroll|code="));Serial.println(p);
    }
    return;
  }

  //FINGERPRINT_OK                      0x00

  p = sensor.image2Tz(2);
  if (p != FINGERPRINT_OK) {
    //FINGERPRINT_IMAGEMESS             0x06
    //FINGERPRINT_PACKETRECIEVEERR      0x01
    //FINGERPRINT_FEATUREFAIL           0x07
    //FINGERPRINT_INVALIDIMAGE          0x15
    //Unknown error
    Serial.print(F("$enroll|code="));Serial.println(p);
    return;
  }

  //FINGERPRINT_OK                      0x00

  p = sensor.createModel();
  if(p != FINGERPRINT_OK) {
    //FINGERPRINT_PACKETRECIEVEERR      0x01
    //FINGERPRINT_ENROLLMISMATCH        0x0A
    //Unknown error
    Serial.print(F("$enroll|code="));Serial.println(p);
    return;
  }

  p = sensor.storeModel(id);
  if (p != FINGERPRINT_OK) {
    //FINGERPRINT_PACKETRECIEVEERR      0x01
    //FINGERPRINT_BADLOCATION           0x0B
    //FINGERPRINT_FLASHERR              0x18
    //Unknown error
    Serial.print(F("$enroll|code="));Serial.println(p);
    return;
  }

  //Fingerprint enrolled successfully
  Serial.print(F("$enroll|id="));Serial.println(id);
  id = 0;
  status = 0;
  sensor.LEDcontrol(false);
}
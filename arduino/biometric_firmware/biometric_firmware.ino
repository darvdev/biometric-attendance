#include <Adafruit_Fingerprint.h>

SoftwareSerial serial(2, 3);
Adafruit_Fingerprint sensor = Adafruit_Fingerprint(&serial);

unsigned long previousMs = 0;
String string = "";
uint8_t status = -1;
uint8_t id = 0;

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
    sensor.getTemplateCount();

    if (sensor.templateCount == 0) {
      status = 102; //No fingerprint registered
      digitalWrite(13, 1);

    } else {
      status = 0; //Standby
      digitalWrite(13, 1);

    }

  } else {
    status = 101; //Sensor not connected/detected

  }

}

void loop() {
  unsigned long ms = millis();

  if (ms - previousMs >= 50)  {
    previousMs = ms;
    
    if (status == 1) {
      startFingerprintAttendance();
    } else if (status == 2) {
      if (id > 0) {
        enrollFingerprintStep1();
      }
    } else if (status == 3) {
      if (id > 0) {
        enrollFingerprintStep2();
      }
    }

  }

}


void serialEvent() {
  while (Serial.available() > 0) {
    char read = (char)Serial.read();

    string.toLowerCase();
    
    if (read == '\n') {

      if (string == "start") {
        sensor.LEDcontrol(true);
        status = 1; //Start reading fingerprint for attendance
        Serial.println(F("start"));

      } else if (string == "status") {
        Serial.print(F("status="));Serial.println(status);

      } else if (string == "enroll") {
        status = 2; //Start reading fingerprint for enrollment
        Serial.println(F("enroll"));

      } else if (string == "standby") {
        status = 0; //Stop sensor operation
        sensor.LEDcontrol(false);
        Serial.println(F("standby"));

      } else {

        if(status == 2 && string.indexOf("id=") > -1) {
          string.replace("id=", "");
          id = string.toInt();
          if (id > 0) {
            sensor.LEDcontrol(true);
            Serial.print("Waiting for valid finger to enroll as #"); Serial.println(id);
          }

        } else {
          Serial.print(F("$msg|e:Unknown command \""));Serial.print(string);Serial.println(F("\""));

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
  switch (p) {
    case FINGERPRINT_OK:
      //Image taken
      break;

    case FINGERPRINT_NOFINGER:
      // Serial.println("No finger detected");
      return;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$start|msg=e:Communication error"));
      return;

    case FINGERPRINT_IMAGEFAIL:
      Serial.println(F("$start|msg=e:Imaging error"));
      return;

    default:
      Serial.println(F("$start|msg=e:Unknown error"));
      return;
  }

  // OK success!

  p = sensor.image2Tz();
  switch (p) {
    case FINGERPRINT_OK:
      // Image converted
      break;

    case FINGERPRINT_IMAGEMESS:
      Serial.println(F("$start|msg=e:Image too messy"));
      return;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$start|msg=e:Communication error"));
      return;

    case FINGERPRINT_FEATUREFAIL:
      Serial.println(F("$start|msg=e:Could not find fingerprint features"));
      return;

    case FINGERPRINT_INVALIDIMAGE:
      Serial.println(F("$start|msg=e:Could not find fingerprint features"));
      return;

    default:
      Serial.println(F("$start|msg=e:Unknown error"));
      return;
  }

  // OK converted!
  p = sensor.fingerSearch();
  if (p == FINGERPRINT_OK) {
    // Found a print match
  } else if (p == FINGERPRINT_PACKETRECIEVEERR) {
    Serial.println(F("$start|msg=e:Communication error"));
    return;

  } else if (p == FINGERPRINT_NOTFOUND) {
    Serial.println(F("$start|msg=e:Did not find a match"));
    return;

  } else {
    Serial.print(F("$start|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
    return;

  }

  // Send sensor data
  Serial.print(F("$start|id="));Serial.print(sensor.fingerID);Serial.print(F(",confidence="));Serial.println(sensor.confidence);
  return;
}


void enrollFingerprintStep1() {

  int p = sensor.getImage();

  switch (p) {
    case FINGERPRINT_OK:
      //Image taken
      break;

    case FINGERPRINT_NOFINGER:
      //No fingerprint
      return;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$enroll|msg=e:Communication error"));
      return;

    case FINGERPRINT_IMAGEFAIL:
      Serial.println(F("$enroll|msg=e:Imaging error"));
      return;

    default:
      Serial.print(F("$enroll|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
      return;
    }

  // OK success!

  p = sensor.image2Tz(1);
  switch (p) {
    case FINGERPRINT_OK:
      //Image converted
      break;

    case FINGERPRINT_IMAGEMESS:
      Serial.println(F("$enroll|msg=e:Image too messy"));
      return;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$enroll|msg=e:Communication error"));
      return;

    case FINGERPRINT_FEATUREFAIL:
      Serial.println(F("$enroll|msg=e:Could not find fingerprint features"));
      return;

    case FINGERPRINT_INVALIDIMAGE:
      Serial.println(F("$enroll|msg=e:Could not find fingerprint features"));
      return;

    default:
      Serial.print(F("$enroll|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
      return;

  }

  //Enroll fingerprint step 1 success

  Serial.println(F("$enroll|msg=i:Remove finger then place same finger again"));
  


  status = 3; //Enroll fingerprint step 2
  
}

void enrollFingerprintStep2() {
  int p = sensor.getImage();
  
  switch (p) {
    case FINGERPRINT_OK:
      //Image taken
      break;

    case FINGERPRINT_NOFINGER:
      //No fingerprint
      return;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$enroll|msg=e:Communication error"));
      return;

    case FINGERPRINT_IMAGEFAIL:
      Serial.println(F("$enroll|msg=e:Imaging error"));
      return;

    default:
      Serial.print(F("$enroll|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
      return;

  }
  // OK success!

  p = sensor.image2Tz(2);
  switch (p) {
    case FINGERPRINT_OK:
      //Image converted
      break;

    case FINGERPRINT_IMAGEMESS:
      Serial.println(F("$enroll|msg=e:Image too messy"));
      return;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$enroll|msg=e:Communication error"));
      return;

    case FINGERPRINT_FEATUREFAIL:
      Serial.println(F("$enroll|msg=e:Could not find fingerprint features"));
      return;

    case FINGERPRINT_INVALIDIMAGE:
      Serial.println(F("$enroll|msg=e:Could not find fingerprint features"));
      return;

    default:
      Serial.print(F("$enroll|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
      return;
  }

  // OK converted!
  Serial.println(F("$enroll|msg=e:Creating fingerprint model"));

  p = sensor.createModel();

  switch (p) {
    case FINGERPRINT_OK:
      //Prints matched!
      break;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$enroll|msg=e:Communication error"));
      return;

    case FINGERPRINT_ENROLLMISMATCH:
      Serial.println(F("$enroll|msg=e:Fingerprints did not match"));
      return;

    default:
      Serial.print(F("$enroll|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
      return;

  }

  p = sensor.storeModel(id);
  switch (p) {
    case FINGERPRINT_OK:
      //Fingerprint stored in id
      break;

    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println(F("$enroll|msg=e:Communication error"));
      return;

    case FINGERPRINT_BADLOCATION:
      Serial.println(F("$enroll|msg=e:Could not store in that location"));
      return;
    
    case FINGERPRINT_FLASHERR:
      Serial.println(F("$enroll|msg=e:Error writing to flash"));
      return;

    default:
      Serial.print(F("$enroll|msg=e:Unknown error \""));Serial.print(p);Serial.println("\"");
      return;

  }
  
  //Fingerprint enrolled successfully
  Serial.print(F("$enroll|id="));Serial.println(id);
  id = 0;
  status = 0;
}
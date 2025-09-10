# FacilityAlarm

Ein **C#-Plugin für SCP: Secret Laboratory** (basierend auf LabAPI und EXILED) zur Steuerung von **Alarmen, Codes und Alarmlichtern** innerhalb der Facility.  
Es ermöglicht Administrator:innen, akustische Warnungen, Codes und visuelle Alarmsignale direkt über Remoteadmin Commands auszuführen.

---

## Features

- **Alarme abspielen**  
  - Vordefinierte Audiofiles (`Alarm1`, `Alarm2`), optional mit Loop.  
  - Start/Stopp via Admin-Befehl.  

- **Codes triggern**  
  - Verschiedene Code-Varianten mit Audio (`Code Red`, `Code Blue`, `Code White1`, `Code White2`, `Code Yellow`, `Code Green`, etc.).  
  - Audiofiles werden automatisch geladen, falls noch nicht im Speicher.  

- **Alarmlichter steuern**  
  - Farbige, pulsierende Beleuchtung in allen Räumen.  
  - Unterstützte Farben: Rot, Blau, Orange, Lila, Gelb, Grün, Navy.  
  - Atmungseffekt (Farbintensität wechselt alle 2 Sekunden).  

- **Hilfefunktion**  
  - `falarm help alarm` → zeigt Alarme und Syntax.  
  - `falarm help code` → listet verfügbare Codes.  
  - `falarm help alarmlight` → zeigt Lichtoptionen.  

---

## Commands

### Alarm
- falarm alarm (Alarmname) [on/off]
- falarm alarm stop
-  **Alarm1**, **Alarm2** verfügbar  
- Optional: `on/off` -> legt fest, ob im Loop wiederholt wird (default: off)

### Code
- falarm code (codename)
- falarm code stop
- Unterstützte Codes: Red, Blue, White1, White2, Orange, Purple1, Purple2, Yellow, Green, Gold, Grey, Maroon, Navy, Black

### Alarmlicht
- falarm alarmlight (color)
- falarm alarmlight stop
- Farben: Red, Blue, Orange, Purple, Yellow, Green, Navy  
- Stop setzt alle Lichter auf schwarz; Reset via `fcolor reset`

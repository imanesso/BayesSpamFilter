# DIST Programmierübung 1: Bayes Spam Filter
FHNW 5Ibb: Diskrete Stochastik 

Team: Jonathan James Bättig, Anessollah Ima

## Ausführung
Das Programm kann auf folgende zwei Arten gestartet werden:
- Ausführung der .exe über den Pfad \BayesSpamFilter\BayesSpamFilter\bin\Release\netcoreapp2.2\win10-x64\BayesSpamFilter.exe
- Ausführung direkt in Visual Studio.

## Resultat
Die Ausführung des Programmes führt zu folgendem Resultat. Wie wir zu diesem Resulat gelangt sind ist im Code dokumentiert. Bei Fragen stehen wir Ihnen gerne zur Verfügung.

### Kalibrierung
- Optimaler Schwellwert -> 0.549999999999999.
- Benutztes Alpha -> 0.000001.
- 31 Ham Mails von total 1338 Mails wurden als Spam markiert.
- 6 Spam Mails von total 209 Mails wurden als Ham markiert.
- Ham Error Rate:  2.3169%.
- Spam Error Rate: 2.8708%.

### Testing
- 36 von 1510 Ham Mails wurden als Spam markiert.
- Error Rate beträgt: 2.3841%.

- 6 von 222 Spam Mails wurden als Ham markiert.
- Error Rate beträgt: 2.7027%.

### Bemerkung
Wir haben uns dazu entschieden, dass wir die Ham uns Spam Fehlerrate möglichst nahe beieinander haben möchten. Deshalb kalibrieren wir solange, bis die prozentualen Anteile der Ham und Spam Fehler möglichst gleich sind. Es gäbe auch noch andere Optionen. Beispielsweise könnte man definieren, dass nur 1% der Ham Mails als Spam identifiziert werden dürfen. Bei uns führte ein solcher Wert jedeoch zu vielen Spam Mails welche als Ham klassifiziert wurden. Dieses Verhalten erachten wir als unerwünscht.
Proof of Concept, ktorý som vytvoril z existujúceho projektu v zadaní:

0. Cieľom PoC je demonštrácia refaktoringu a možností rozvoja daného projektu.
Na demonštráciu som si vybral okno ProductList.
Vo vytvorenom PoC je možné porovnať pôvodnú architektúru s rozšírením v zmysle Clean Architesture Pattern.

1. Vytvoril som separátny projekt Invoice.DB.
Tento obsahuje model v Entity Framework, ktorý zastrešuje CRUD operácie a perzistenciu údajov.
Jeho účelom je odčlenenie prístupu k databáze od vyšších vrstiev, ktoré s údajmi pracujú iba na objektovej úrovni.

2. Vytvoril som projekt WebApi.
Tento obsahuje Controller triedy pre prístup k údajom volaním REST API služieb.
Jeho účelom je vznik API rozhrania použiteľného pre rôzne typy konzumentov služieb, napríklad tučný klient, tenký klient, services a pod.

3. V okne ProductList som pridal možnosť čítania a zápisu údajov objektami z InvoiceDB. Pôvodne sa s údajmi pracovalo pomocou SQL príkazov.
Na ilustráciu som do okna ProductList umiestnil 3 radiobuttony, ktoré menia logiku čítania a zápisu údajov: Bud pôvodným spôsobom, alebo priamym volaním Invoce.DB, alebo pomocou REST API.

4. Unit test
Príklad unit testu, ktorý modifikuje údaje priamym volaním Invoice.DB a testuje modifikácie volaním REST API.

Co som neriešil:
1. Zmena databázy z lokálnej DB na MS SQL server alebo Oracle.
Toto som vynechal pre zjednodušenie distribúcie PoC a tiež preto, lebo po migrácii údajov je zmena iba v connect stringu.

2. Parametre v config súboroch
Pre urýchlenie vývoja PoC som priamo v zdrojovom kode použil konštanty

3. Komplexná prerábka
Zmena je iba v okne ProductList, na účely demonštrácie je to postačujúce, prerábka ostatných use cases by bola analogická.

4. Cistota kodu
Mnou vytvorený kod nesplňa všetky best practises pre čistotu kodu, StyleCop, FxCop a iné pravidlá. Nepovažoval som to za relevantné z pohľadu PoC.

5. Odstraňovanie chýb
Pôvodný zdrojový kod je na viacerých miestach chybový a málo koncepčný, nie sú správne ošetrené chybové stavy a podobne. Tento aspekt nie je relevantný z pohľadu PoC.

Námety na další rozvoj:
1. Perzistencia údajov v databázovom serveri (MS SQL, Oracle)
2. Doplniť do dátového modelu primárne a cudzie kľúče, indexy a pod.
3. Nastavenia presunúť do config súborov
4. Ošetriť aplikáciu na možné exceptions
5. Implementovať logovanie
6. Prerobiť .NET WebApi na .NET Core
7. Doplniť validácie údajov na vstupoch
8. Web klient, prípadne iné
9. Chrániť údaje prístupovmi právami
10. Pokryť unit testami všetký use cases
11. Infraštruktúrne otázky - CI/CD, Deploy, verzovanie, ...
12. Dokumentácia (používateľská, technická)

Spustenie Poc:
1. Vo všetkých *.config súboroch nastaviť správnu cestu k lokálnej DB v sekcii connectionStrings
2. Naštartovanie WebApi: Build projektu WebApi, spustenie ako WebApi.exe

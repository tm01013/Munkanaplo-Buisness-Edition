# Hogyan használd a Munkanaplót?

> Az útmutató során végigvesszük a Munkanapló minden funkcióját és menedzser illetve beosztott szemszögből is megnézzük azokat.

<details>
<summary><h2>1. Regisztráció</h2></summary>
<hr>

> A konfiguráció *USE_MANAGERS* opciója befolyásolja ezt!

### Beosztottak:

- Regisztrálásnál a valódi neveteket használjátok!
- Ehhez nem szükséges e-mail cím vagy telefonszám.

### Menedzserek:

- Regisztrálásnál a valódi neveteket használjátok!
- Ehhez nem szükséges e-mail cím vagy telefonszám.

</details>

<details>
<summary><strong><h2>2. Projektek</h2></strong></summary>
<hr>

> A konfiguráció *USE_MANAGERS* opciója befolyásolja ezt az egységet!

- A Projekt a legnagyobb egység a munkanaplóban.

### Beosztottak:

- Projekteket csak a menedzserek hozhatnak létre.
- Egy meglévő projektbe is a menedzseretek tud meghívni.
- A meglévő projekteket könnyedén elérhetitek a menüsorban a nevére kattintva (bal oldalon)

### Menedzserek:

- Egy új projektet a _Projektek_ oldalon tudtok létrehozni.
- Egy meglévő projekthez a projekt létrehozója tud meghívni a *Tagok szerkesztése* gombra kattintva
- A meglévő projekteket könnyedén elérhetitek a menüsorban a nevére kattintva (bal oldalon)

<br>

- **Figyelem! Egy projekt törlésével az ahoz tartozó összes adat véglegesen törlődik!**

</details>

<details>
<summary><h2>3. Feladatok</h2></summary>
<hr>

- **Egy feladatokhoz csak a projekt tagjai férhetnek hozzá**

### Beosztottak:

- Egy projekthez tartozó feladatokat két féle módon tekinthetitek meg:<br>
  1. Rakattintasz a menüsorban a projekt nevére
  2. A _projektek_ menüben rékattintotok a _Feladatok_ gombra
- Feladatokat itt tudtok létrehozni, szerkeszteni és nézegetni

- Mikor egy feladatot elvégeztetek kattintsatok a _KÉSZ_ gombra.
  > Ekkor a feladat státusza _befelyezett_ lesz.<br>
  > A program azt is nyilvántartja hogy **ki** és mikor felyezte be a feladatot!<br>
  > Ez nem vonható vissza!

### Menedzserek:

- Egy projekthez tartozó feladatokat két féle módon tekinthetitek meg:<br>
  1. Rakattintasz a menüsorban a projekt nevére
  2. A _projektek_ menüben rékattintotok a _Feladatok_ gombra
- Feladatokat itt tudtok létrehozni, szerkeszteni és nézegetni

- A Menedzserek nem felyezhetnek be feladatoket!
</details>

<details>
<summary><h2>4. Alfeladatok</h2></summary>
<hr>

- Egy alfeladat kizérólag egy párszavas cimmel és egy egysoros leírással rendelkezik.

> Ez csak kísérleti funkció

### Beosztottak:

- Alfeladatokat egy feladat _részletek_ oldaláról hozhattok létre
- Alfeladatokat egy feladat _szerkesztés_ oldaláról törölhettek.

### Menedzserek:

- A Menedzserek nem hozhatnak létre, nem szerkeszthetik és törölhetik az alfeladatokat
</details>

<details>
<summary><h2>5. Munka</h2></summary>
<hr>

> Végre eljutottunk a lényeghez ;)

### Beosztottak:

- Egy munkát a feladat *részletek* oldaláról tudtok elindítani
  > A megnyíló  *munka folyamatban* ablakot bezárhatjátok, etől a munka nem fog leállni.
- Egyszerre csak egy munkát tudtok indítani profilonként

- A jelenleg folyamatban lévő munkát a menüsor jobb oldalán lévő táska ikonra kattintva érhettek el.

- A befejezett munkáidat a *Munkáim* gombra (menüsor jobboldal) kattinta érhettek el
- A munkáim oldalon a kereseteiteket is látjátok
  > Ha a menedzseretek beállította az órabéreteket

### Menedzserek:

- A beosztottjaitokat a *beosztottjaim* gombra (menüsor jobb oldal) kattintva érhetitek el 
  > Valaki csak akkor számit a beosztottadnak, ha legalább egy olyan olyan projektneki is a tagja aminek te is tagja vagy
- A beosztottaid órabérét is a *beusztottjaim* oldalon szerkesztheted
- Itt tudod a beosztottjaid munkáit is megtekinteni
  > A beosztattjaid csak azokhoz a projektekhez tartozó munkáit léthatod amelyeknek te is tagja vagy.
</details>

<details>
<summary><h3>Konfiguráció</h3></summary>
<hr>

- A konfigurációt egy *.env* fájlba kell rakni!
- Alapértelmezett konfiguráció:
  ```conf
  USE_MANAGERS="true"
  ADMIN_USERNAME=""
  ```
- **A konfigurációnak ilyen formában kell lennie!**
- **Csak az idézőjeleken belüli részt változtassátok meg!**

#### USE_MANAGERS
- Ez a változó kizárólag *true* vagy *false* értéket vehet fel!
- Ez az opció változtatja, hogy vannak-e menedzserek.

Ha a változót *false* -ra állítod ezek változnak a fent leírtakhoz képest:
  - Nem lehet *menedzserként* regisztrálni
  - Bárki hozhat létre projekteket
  - A beosztottak nézethez csak az admin felhasználó fér hozzá.
  - Csak az admin felhasználó szerkesztheti az órabéreket

#### ADMIN_USERNAME
- Ez az opció cak akkor lesz figyelembe véve ha a *USE_MANAGERS* opció *false*-ra van állítva
  > Attól még hogy a *USE_MANAGERS* *true*-ra van állítva a konfigurációnak tartalmaznia kell!
- Abban az esetben ha a menedzserek ki vannak kapcsolva ez határozza meg ki az admin felhasználó (akinek ezzel eggyezik a felhasználóneve)
- A kis és nagbetűk számítanak!


</details>

## Most már mindent tudtok :)
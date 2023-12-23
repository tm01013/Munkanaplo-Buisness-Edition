# Munakanpló *Buisness Edition*

Egy nyílt forráskódú munkanapló vezetését megkönnyítő alkalmazás

<details>
<summary><strong><h2>Telepítése</h2></strong></summary>
<hr>

### Rendszerkövetelmények

- Docker 20+

### Futtatása

> Ehhez adminisztrátori jogosultságra van szükség!

1. Docker kép letöltése:
    ```bash
    sudo docker pull tm01013/munkanaplo-be
    ```

2. Szerver indítása:
    ```bash
    sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo-be
    ```
3. Konfiguráció hozzáadása (kötelező)
    ```bash
    sudo docker cp <.env fájl> Munkanaplo:/app/.env
    ```
    - A konfigurációs fájlnak (.env) a következő beállításokat kell tartalmaznia:
        ```conf
        USE_MANAGERS="true"
        ADMIN_USERNAME=""
        ```
    - ezek hatásairól [itt](HOWTOUSE.md) olvashtsz részletesen
    

<details>
<summary><h3>Frissítés korábbi verzióról</h3></summary>

> Ehhez adminisztrátori jogosultságra van szükség!

1. Adatbázis és konfiguráció kimásolása a régi konténerből
    ```bash
    sudo docker cp <régi konténer neve>:/app/app.db ~/app.db
    sudo docker cp <régi konténer neve>:/app/.env ~/.env
    ```
2. Régi konténer törlése
    ```bash
    sudo docker remove <régi konténer neve>
    ```
3. Új konténer telepítése
    ```bash
    sudo docker pull tm01013/munkanaplo-be
    sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo-be
    ```
4. Adatbázis bemásolása az új konténerbe
    ```bash
    sudo docker cp ~/app.db Munkanaplo:/app/app.db
    sudo docker cp ~/.env Munkanaplo:/app/.env
    ```

</details>

<details>
<summary><h4>Telepítés forráskódból</h4></summary>

> Ehhez adminisztrátori jogosultságra van szükség!

1. Projekt klonolása:
    ```bash
    git clone https://github.com/tm01013/Munkanaplo-Buisness-Edition.git
    cd Munkanaplo2
    ```

2. Microsoft aspnet és dotnet sdk letöltése:
    ```bash
    sudo docker pull mcr.microsoft.com/dotnet/aspnet:7.0
    sudo docker pull mcr.microsoft.com/dotnet/sdk:7.0.401
    ```

3. Docker kép készítése
    ```bash
    sudo docker build -t munkanaplo-be --no-cache .
    ```

4. Szerver indítása
    ```bash
    sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo-be
    ```
5. Konfiguráció hozzáadása (kötelező)
    ```bash
    sudo docker cp <.env fájl> Munkanaplo:/app/.env
    ```
    - A konfigurációs fájlnak (.env) a következő beállításokat kell tartalmaznia:
        ```conf
        USE_MANAGERS="true"
        ADMIN_USERNAME=""
        ```
    - ezek hatásairól [itt](HOWTOUSE.md) olvashtsz részletesen

</details>

</details>

## [Használata](/HOWTOUSE.md)

## [Licence](/LICENCE)

© Tatár Márton 2023

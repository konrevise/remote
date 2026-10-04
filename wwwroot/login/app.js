import { getElements } from "/services/html.js";

const elementsNames =
[
	"loginButton",
    "stateLabel",
	"nameInput",
	"passwordInput"
];

const elements = getElements(elementsNames);

import { sendJson, postMethod } from "/services/sending.js";

import { authenticationToken } from "/services/storage.js";
import { goToInternal } from "/services/location.js";

async function initiateLoginChallenge(userName)
{
    const dataToSend = 
    {
        name: userName,
    };

    const response = await sendJson(
        dataToSend, "app/initiateLoginChallenge", postMethod);
    const contentType = response.headers.get('content-type');

    let result;

    if (contentType && contentType.includes('application/json'))
    {
        const responseResult = await response.json();
        console.log("tried to initiate login challenge:");
        console.log(responseResult);

        result =
        {
            content: responseResult,
            success: true
        };
        return result;
    }
    else if (contentType && contentType.includes('text/html'))
    {
        const responseResult = await response.text();
        result =
        {
            content: responseResult,
            success: false
        };
    }

    return result;
}

// Вспомогательные функции для кодирования (Hex <-> Bytes)
const hexToBytes = hex =>
    new Uint8Array(hex.match(/.{1,2}/g).map(byte => parseInt(byte, 16)));
const bytesToHex = bytes =>
    Array.from(bytes).map(b => b.toString(16).padStart(2, '0')).join('');

function stringToBytes(str)
{
    const encoder = new TextEncoder();
    return encoder.encode(str); // Возвращает Uint8Array
}

// Принимает: str (строка пароля), salt (Uint8Array байты), iterations (число)
// Возвращает: Хэш пароля в виде Hex-строки
async function stringToPBKDF2(str, salt, iterations)
{
    const strBytes = stringToBytes(str);

    const baseKey = await crypto.subtle.importKey(
        "raw", strBytes, { name: "PBKDF2" }, false, ["deriveBits"]
    );

    const derivedBits = await crypto.subtle.deriveBits(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: iterations,
            hash: "SHA-256"
        },
        baseKey,
        256
    );

    return bytesToHex(new Uint8Array(derivedBits));
}


async function HMAC(keyHex, messageHex)
{
    const keyBytes = hexToBytes(keyHex);
    const messageBytes = hexToBytes(messageHex);

    const hmacKey = await crypto.subtle.importKey(
        "raw", keyBytes, { name: "HMAC", hash: "SHA-256" }, false, ["sign"]);

    const sig = await crypto.subtle.sign("HMAC", hmacKey, messageBytes);

    return bytesToHex(new Uint8Array(sig));
}

async function finishLoginChallenge(challengeID, oneTimeKey)
{
    const dataToSend = 
    {
        challengeID: challengeID,
        oneTimeKey: oneTimeKey
    };

    const response = await sendJson(
        dataToSend, "app/finishLoginChallenge", postMethod);
    const contentType = response.headers.get('content-type');

    let result;

    if (contentType && contentType.includes('application/json'))
    {
        const responseResult = await response.json();
        console.log("tried to finish login challenge:");
        console.log(responseResult);

        result =
        {
            content: responseResult,
            success: true
        };
        return result;
    }
    else if (contentType && contentType.includes('text/html'))
    {
        const responseResult = await response.text();
        result =
        {
            content: responseResult,
            success: false
        };
    }

    return result;
}

elements["loginButton"].addEventListener('click', async () =>
{
    console.log("initiate login challenge");
    const userName = elements["nameInput"].value;
    const initiativeResult = await initiateLoginChallenge(userName);

    if (!initiativeResult.success)
    {
        console.log(`error: ${initiativeResult.content}`);
        elements["stateLabel"].textContent = initiativeResult.content;
        return;
    }

    console.log("convert the password to it's hash");
    const password = elements["passwordInput"].value;
    const salt = initiativeResult.content.hashSalt;
    const iterations = initiativeResult.content.hashIterations;
    const passwordHash = await stringToPBKDF2(
        password, hexToBytes(salt), iterations);
    
    console.log("make a one-time key");
    const nonce = initiativeResult.content.nonce;
    const oneTimeKey = await HMAC(passwordHash, nonce);

    console.log("finish login challenge");
    const challengeID = initiativeResult.content.challengeID;
    const finishResult = await finishLoginChallenge(challengeID, oneTimeKey);

    if (!finishResult.success)
    {
        console.log(`error: ${finishResult.content}`);
        elements["stateLabel"].textContent = finishResult.content;
        return;
    }

    console.log("get authentication token");
    authenticationToken.value = finishResult.content.authenticationToken;
    goToInternal("mainMenu");
});

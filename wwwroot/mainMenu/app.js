import { getElements } from "/services/html.js";

const elementsNames =
[
	"logoutButton"
];

const elements = getElements(elementsNames);

import { sendJson } from "/services/sending.js";
import { authenticationToken } from "/services/storage.js";
import { goToInternal } from "/services/location.js";

elements["logoutButton"].addEventListener('click', async () =>
{
    const dataToSend = 
    {
    	authToken: authenticationToken.value
    };

    const response = await sendJson(dataToSend, "app/logout", "POST");

    authenticationToken.value = "";

    goToInternal("");
});
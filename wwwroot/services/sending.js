import { authenticationToken } from "/services/storage.js";

const domain = "https://192.168.0.108:8080";

const successText = "success!"
const clientErrorText = "a client error occured!";
const serverErrorText = "a server error occured!";

export const postMethod = "POST";

export async function sendJson(data, apiCommand, httpMethod)
{
	let response = null;

    try
    {
    	response = await fetch(
    		`${domain}/api/${apiCommand}`,
	    	{
	    		method: httpMethod,
	    		headers:
	    		{
	    			"Authorization": `Bearer ${authenticationToken.value}`,
	    			"Content-Type": "application/json"
	        	},
	        	body: JSON.stringify(data)
	    	}
    	);
	}
	catch (error)
	{
		console.log(`an error occured while sendJson: ${response}`);
	}

    return response;
}

import { sendJson } from "/services/sending.js";
import { authenticationToken } from "/services/storage.js";
import { goToInternal } from "/services/location.js";

async function getGuardInstructions()
{
	const currentUrl = window.location.href;

	const dataToSend =
	{
		authenticationToken: authenticationToken.value,
		url: currentUrl
	}

	const response = await sendJson(
		dataToSend, "app/getGuardInstructions", "POST");

	if (response != null && response.ok)
	{
		const result = await response.json();
		console.log("got guard instructions:");
		console.log(result);
		return result;
	}
	else
	{
		return null;
	}
}

function performGuardInstructions(instructions)
{
	const destinationUrl = instructions.destinationUrl;

	const redirect = destinationUrl != "none";
	console.log(`did guard tell me to redirect? ${redirect}`);
	if (redirect)
	{
		window.location.replace(destinationUrl);
	}
}

async function main()
{
	const instructions = await getGuardInstructions();
	if (instructions == null) goToInternal("login");
	
	performGuardInstructions(instructions);
}

main();

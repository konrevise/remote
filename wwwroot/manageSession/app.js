import { sendJson } from "/services/sending.js";

document.addEventListener('click', async (event) =>
{
    const button = event.target.closest('.actionButton');
    if (!button) return;

    const commandID = +button.dataset.command_id;

    const dataToSend =
    {
        commandID: commandID,
    };

    await sendJson(dataToSend, "main/session", "POST");
});

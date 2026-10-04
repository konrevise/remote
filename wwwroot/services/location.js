const domain = "https://192.168.0.108:8080";

export function getLink(path)
{
	return `${domain}/${path}`;
}

export function goToInternal(path)
{
	window.location.replace(getLink(path));
}

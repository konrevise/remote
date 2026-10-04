export function getElements(elementsNames)
{
	let elements = {};

	for (let i = 0; i < elementsNames.length; i++) {
		let name = elementsNames[i];
		elements[name] = document.getElementById(name);
	}
	return elements;
}

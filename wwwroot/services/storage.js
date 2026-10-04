export let authenticationToken =
{
	get value()
	{
		return localStorage.getItem('authenticationToken');
	},

	set value(newValue)
	{
		localStorage.setItem('authenticationToken', newValue);
	}
}

(function() {
	if (!window["HatunSearch"]) HatunSearch = { };
	if (!HatunSearch.PartnersWeb) HatunSearch.PartnersWeb = { };
	HatunSearch.PartnersWeb.Accounts = {
		SignupStep3: { Start: function(countryAPI) { HatunSearch.PartnersWeb.Shared.RegisterGeoDataCombos(countryAPI); } }
	};
})();
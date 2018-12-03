(function() {
	if (!window["HatunSearch"]) HatunSearch = { };
	if (!HatunSearch.PartnersWeb) HatunSearch.PartnersWeb = { };
	HatunSearch.PartnersWeb.Management = { };
	HatunSearch.PartnersWeb.Management.Account = { };
	HatunSearch.PartnersWeb.Management.Account.AddCard = {
		Start: function(language, stripePublishableKey, baseUrl) {
			var stripe = Stripe(stripePublishableKey);
			var elements = stripe.elements({
				fonts: [
					{
						family: "Roboto",
						src: "url('" + baseUrl + "ttf/Roboto.400.ttf')",
						weight: "normal"
					},
					{
						family: "Roboto",
						src: "url('" + baseUrl + "ttf/Roboto.500.ttf')",
						weight: "bold"
					}
				]
			});
			var card = elements.create("card", {
				hidePostalCode: true,
				style: {
					base: {
						fontFamily: "Roboto",
						fontSize: "16px"
					}
				}
			});
			card.mount("#txtTokenId");
			var form = document.querySelector("form");
			form.onsubmit = function(event) {
				event.preventDefault();
				stripe.createToken(card).then(function(result) {
					if (!result.error) {
						var input = document.createElement("input");
						input.setAttribute("name", "TokenId");
						input.setAttribute("type", "hidden");
						input.setAttribute("value", result.token.id);
						form.appendChild(input);
						form.submit();
					}
					else {
						var content = null, header = null;
						if (language === "ES") {
							header = "Error al añadir la tarjeta de crédito";
							content = "Stripe no ha podido reconocer la tarjeta de crédito ingresada. Intente con otra tarjeta de crédito.";
						}
						var dialog = MaterialDesign.Dialogs.Create("confirmation", {
							header: header,
							content: content,
							language: language,
							hasAnOkButton: true,
							okHandler: function(dialog) { MaterialDesign.Dialogs.Hide(dialog); }
						});
						MaterialDesign.Dialogs.Show(dialog);
					}
				});
			}
		}
	};
	HatunSearch.PartnersWeb.Management.Account.EditCompanyInfo = { Start: function(countryAPI) { HatunSearch.PartnersWeb.Shared.RegisterGeoDataCombos(countryAPI); } };
	HatunSearch.PartnersWeb.Management.Home = {
		Start: function() {
			var date = new Date();
			var hour = date.getHours(), timeofday = "morning";
			if (hour > 12 && hour < 19) timeofday = "afternoon";
			else if (hour >= 19 || hour < 6) timeofday = "night";
			var banner = document.querySelector("md\\:paper:first-of-type");
			banner.setAttribute("timeofday", timeofday);
		}
	};
	HatunSearch.PartnersWeb.Management.Sales = { };
	HatunSearch.PartnersWeb.Management.Sales.AddProperty = { Start: function(countryAPI) { HatunSearch.PartnersWeb.Shared.RegisterGeoDataCombos(countryAPI); } };
})();
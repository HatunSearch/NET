(function() {
	if (!window["HatunSearch"]) HatunSearch = { };
	if (!HatunSearch.PartnersWeb) HatunSearch.PartnersWeb = { };
	HatunSearch.PartnersWeb.Shared = {
		RegisterGeoDataCombos: function(countryAPI) {
			var cboCountry = document.getElementById("cboCountry"), cboRegion = document.getElementById("cboRegion"), cboProvince = document.getElementById("cboProvince"),
				cboDistrict = document.getElementById("cboDistrict");
			var txtRegion = cboRegion.parentElement, txtProvince = cboProvince.parentElement, txtDistrict = cboDistrict.parentElement;
			var latestXHRCountry = null, latestXHRRegion = null, latestXHRProvince = null;
			var addOptionToCombo = function(combo, name, value) {
				var option = document.createElement("option");
				option.setAttribute("value", value);
				option.innerText = name;
				combo.appendChild(option);
			}, cleanCombo = function(textfield, combobox) {
				textfield.setAttribute("isActive", false);
				var firstChild = null;
				while (firstChild = combobox.firstChild) combobox.removeChild(firstChild);
			}, httpHelper = function(url, callback) {
				var xhr = new XMLHttpRequest();
				xhr.open("GET", url, true);
				xhr.onreadystatechange = function() {
					if (xhr.readyState === 4) {
						var status = xhr.status;
						callback(status, status === 200 ? JSON.parse(xhr.responseText) : xhr.responseText);
					}
				};
				xhr.send();
				return xhr;
			};
			cboCountry.oninput = function() {
				cleanCombo(txtRegion, cboRegion);
				cleanCombo(txtProvince, cboProvince);
				cleanCombo(txtDistrict, cboDistrict);
				if (latestXHRCountry != null && latestXHRCountry.readyState != 4) latestXHRCountry.abort();
				latestXHRCountry = httpHelper(countryAPI + "/" + cboCountry.value + "/regions", function(status, result) {
					if (status === 200) {
						for (var i = 0; i < result.length; i++) {
							var region = result[i], regionValue = region.Country + "|" + region.Code;
							addOptionToCombo(cboRegion, region.DisplayName, regionValue);
						}
						txtRegion.removeAttribute("isActive");
						cboRegion.oninput();
					}
				});
			};
			cboRegion.oninput = function() {
				var regionValue = cboRegion.value;
				regionValue = regionValue.split("|")[1];
				cleanCombo(txtProvince, cboProvince);
				cleanCombo(txtDistrict, cboDistrict);
				if (latestXHRRegion != null && latestXHRRegion.readyState != 4) latestXHRRegion.abort();
				latestXHRRegion = httpHelper(countryAPI + "/" + cboCountry.value + "/regions/" + regionValue + "/provinces", function(status, result) {
					if (status === 200) {
						for (var i = 0; i < result.length; i++) {
							var province = result[i], provinceValue = province.Country + "|" + province.Code;
							addOptionToCombo(cboProvince, province.DisplayName, provinceValue);
						}
						txtProvince.removeAttribute("isActive");
						cboProvince.oninput();
					}
				});
			};
			cboProvince.oninput = function() {
				var regionValue = cboRegion.value, provinceValue = cboProvince.value;
				regionValue = regionValue.split("|")[1];
				provinceValue = provinceValue.split("|")[1];
				cleanCombo(txtDistrict, cboDistrict);
				if (latestXHRProvince != null && latestXHRProvince.readyState != 4) latestXHRProvince.abort();
				latestXHRProvince = httpHelper(countryAPI + "/" + cboCountry.value + "/regions/" + regionValue + "/provinces/" + provinceValue + "/districts", function(status, result) {
					if (status === 200) {
						for (var i = 0; i < result.length; i++) {
							var district = result[i], districtValue = district.Country + "|" + district.Code;
							addOptionToCombo(cboDistrict, district.DisplayName, districtValue);
						}
						txtDistrict.removeAttribute("isActive");
					}
				});
			};
		}
	};
})();
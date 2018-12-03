(function() {
	MaterialDesign = {
		Dialogs: {
			Create: function(type, parameters) {
				var scrim = document.createElement("md:scrim"), dialog = document.createElement("md:dialog");
				var result = { Scrim: scrim, Dialog: dialog, ParentElement: parameters.parentElement };
				dialog.setAttribute("isFullscreen", false);
				dialog.setAttribute("isOpened", false);
				dialog.setAttribute("type", type);
				if (parameters.header) {
					var dheader = document.createElement("md:dheader");
					dheader.innerText = parameters.header;
					dialog.appendChild(dheader);
				}
				var dcontentarea = document.createElement("md:dcontentarea"), dactionarea = document.createElement("md:dactionarea");
				if (parameters.options) {
					var baseId = parameters.baseId, options = parameters.options, value = parameters.value;
					dcontentarea.setAttribute("hasShortMargins", true);
					for (var i = 0; i < options.length; i++) {
						var option = options.item(i);
						var input = document.createElement("input"), label = document.createElement("label");
						var inputId = baseId + "_rb" + i;
						input.id = inputId;
						input.setAttribute("name", baseId);
						input.setAttribute("type", "radio");
						var optionValue = option.value;
						input.setAttribute("value", optionValue);
						if (optionValue == value) input.setAttribute("checked", "checked");
						label.setAttribute("for", inputId);
						label.innerText = option.innerText;
						dcontentarea.appendChild(input);
						dcontentarea.appendChild(label);
					}
				}
				else if (parameters.content) {
					var body = document.createElement("md\:body1");
					dheader.setAttribute("hasShortMargins", true);
					body.innerText = parameters.content;
					dcontentarea.appendChild(body);
				}
				dactionarea.setAttribute("alignment", "horizontal");
				var language = parameters.language;
				if (parameters.hasAnOkButton) {
					var dbutton = document.createElement("md:dbutton"), button = document.createElement("button");
					if (language === "ES") button.innerText = "Aceptar";
					button.onclick = (function(handler, dialog) { return function() { handler(dialog); }; })(parameters.okHandler, result);
					dbutton.appendChild(button);
					dactionarea.appendChild(dbutton);
				}
				if (parameters.hasACancelButton) {
					var dbutton = document.createElement("md:dbutton"), button = document.createElement("button");
					if (language === "ES") button.innerText = "Cancelar";
					button.onclick = (function(handler, dialog) { return function() { handler(dialog); }; })(parameters.cancelHandler, result);
					dbutton.appendChild(button);
					dactionarea.appendChild(dbutton);
				}
				dialog.appendChild(dcontentarea);
				dialog.appendChild(dactionarea);
				scrim.appendChild(dialog);
				return result;
			},
			Hide: function(dialog) {
				var scrim = dialog.Scrim;
				dialog.Dialog.setAttribute("isOpened", false);
				setTimeout(function() {
					scrim.setAttribute("isOpened", false);
					scrim.parentElement.removeChild(scrim);
				}, 100);
			},
			Show: function(dialog) {
				var app = document.querySelector("md\\:app");
				if (app) {
					app.appendChild(dialog.Scrim);
					dialog.Scrim.setAttribute("isOpened", true);
					setTimeout(function() { dialog.Dialog.setAttribute("isOpened", true); }, 16);
				}
			}
		},
		RegisterAppBar: function() {
			var backButton = document.querySelector("md\\:abbutton[action='back']"), menuButton = document.querySelector("md\\:abbutton[action='menu']");
			if (menuButton) {
				var navigationDrawer = document.querySelector("md\\:navigationDrawer"), scrim = navigationDrawer.parentElement;
				var closeNavigationDrawer = function() {
					navigationDrawer.setAttribute("isOpened", false);
					setTimeout(function() {
						document.body.removeAttribute("isScrollEnabled");
						scrim.setAttribute("isOpened", false);
					}, 200);
				};
				navigationDrawer.onclick = function(event) { event.stopPropagation(); };
				menuButton.onclick = function() {
					document.body.setAttribute("isScrollEnabled", false);
					scrim.setAttribute("isOpened", true);
					setTimeout(function() { navigationDrawer.setAttribute("isOpened", true); }, 16);
				};
				var nditems = navigationDrawer.querySelectorAll("md\\:nditem");
				for (var i = 0; i < nditems.length; i++) {
					var nditem = nditems.item(i);
					nditem.onclick = function() {
						closeNavigationDrawer();
						location.href = this.getAttribute("urlaction");
					};
				}
				scrim.onclick = closeNavigationDrawer;
			}
			else if (backButton) { backButton.onclick = function() { location.href = backButton.getAttribute("urlaction"); }; }
		},
		RegisterFloatingActionButtons: function() {
			var fabs = document.querySelectorAll("md\\:floatingactionbutton[urlaction]");
			for (var i = 0; i < fabs.length; i++) {
				var fab = fabs.item(i);
				fab.onclick = function() { location.href = this.getAttribute("urlaction"); };
			}
		},
		RegisterLists: function() {
			var libuttons = document.querySelectorAll("md\\:libutton[urlaction]"), litems = document.querySelectorAll("md\\:litem[urlAction]:not([isDisabled='true'])");
			for (var i = 0; i < libuttons.length; i++) {
				var libutton = libuttons.item(i);
				libutton.onclick = function(event) {
					event.preventDefault();
					event.stopPropagation();
					location.href = this.getAttribute("urlaction");
				};
			}
			for (var i = 0; i < litems.length; i++) {
				var litem = litems.item(i);
				litem.onclick = function() { location.href = this.getAttribute("urlaction"); };
			}
		},
		Snackbars: {
			Show: function(text) {
				var app = document.querySelector("md\\:app"), snackbar = document.createElement("md:snackbar");
				app.appendChild(snackbar);
				setTimeout(function() {
					snackbar.setAttribute("isOpened", true);
					snackbar.innerText = text;
					setTimeout(function() {
						snackbar.setAttribute("isOpened", false);
						setTimeout(function() { app.removeChild(snackbar); }, 100);
					}, 4000);
				}, 16);
			}
		},
		TransformTextfields: function(language) {
			var textfields = document.getElementsByTagName("md:textfield");
			for (var i = 0; i < textfields.length; i++) {
				var textfield = textfields.item(i);
				var input = textfield.querySelector("input");
				if (input) {
					input.oninput = function() {
						var value = this.value;
						if (value) this.setAttribute("value", this.value);
						else this.removeAttribute("value");
					};
				}
				var select = textfield.querySelector("select");
				if (select) {
					var label = textfield.querySelector("label");
					if (label) {
						select.onkeydown = function(event) {
							if (event.code === "Space") {
								event.preventDefault();
								this.click();
							}
						};
						select.onmousedown = function(event) { event.preventDefault(); };
						select.onclick = (function(select) {
							return function() {
								var dialogs = MaterialDesign.Dialogs;
								var dialog = dialogs.Create("confirmation", {
									baseId: select.id,
									cancelHandler: MaterialDesign.Dialogs.Hide,
									header: label.innerText,
									hasACancelButton: true,
									hasAnOkButton: true,
									language: language,
									okHandler: function(dialog) {
										var selectedOption = dialog.Dialog.querySelector("input[type='radio']:checked");
										var select = dialog.ParentElement;
										select.value = selectedOption.value;
										var selectOnInput = select.oninput;
										if (selectOnInput) selectOnInput();
										MaterialDesign.Dialogs.Hide(dialog);
									},
									options: this.options,
									parentElement: select,
									value: this.value
								});
								dialogs.Show(dialog);
							};
						})(select);
					}
				}
				var textarea = textfield.querySelector("textarea");
				if (textarea) {
					var helperText = textarea.querySelector("md\\:helpertext");
					if (!helperText) textfield.style.height = textfield.scrollHeight + "px";
					textarea.oninput = function() {
						var value = this.value;
						if (value) this.setAttribute("value", this.value);
						else this.removeAttribute("value");
					};
					if (textarea.value) textarea.setAttribute("value", textarea.value);
				}
			}
		},
		Start: function(language) {
			this.RegisterAppBar();
			this.RegisterFloatingActionButtons();
			this.RegisterLists();
			this.TransformTextfields(language);
		}
	};
})();
(function (window, undefined) {
  // Global variable to store text list from client
  var textDataList = [];

  window.Asc.plugin.init = function init() {
    // Listen for postMessage from parent window
    setupPostMessageListener();

    // Notify parent window that plugin is ready
    notifyPluginReady();

    // Initialize UI
    initializeUI();
  };

  // Initialize the plugin UI
  function initializeUI() {
    window.Asc.plugin.executeMethod("ActivateWindow", [
      "iframe_asc.{5F2A8B4C-9D3E-4A7B-8C1F-2E9A7B4C5D6F}",
    ]);

    // Populate the selector with current data
    populateTextSelector();

    // Setup autocomplete functionality
    setupAutocomplete();
  }

  // Setup autocomplete functionality
  function setupAutocomplete() {
    var input = document.getElementById("textSelector");
    var dropdown = document.getElementById("dropdownList");
    var selectedIndex = -1;
    var filteredData = [];

    if (!input || !dropdown) {
      console.error("Autocomplete elements not found");
      return;
    }

    // Handle input changes (search functionality)
    input.addEventListener("input", function () {
      var searchTerm = input.value.toLowerCase();
      filteredData = textDataList.filter(function (item) {
        var formattedItem = formatItemDisplay(item);
        return formattedItem.searchText.toLowerCase().includes(searchTerm);
      });

      selectedIndex = -1;
      updateDropdown(filteredData, searchTerm);
    });

    // Initialize dropdown with all items
    filteredData = textDataList.slice(); // Copy array
    selectedIndex = -1;
    updateDropdown(filteredData, "");

    // Handle keyboard navigation
    input.addEventListener("keydown", function (e) {
      if (filteredData.length === 0) return;

      switch (e.key) {
        case "ArrowDown":
          e.preventDefault();
          selectedIndex = Math.min(selectedIndex + 1, filteredData.length - 1);
          updateSelection();
          break;

        case "ArrowUp":
          e.preventDefault();
          selectedIndex = Math.max(selectedIndex - 1, -1);
          updateSelection();
          break;

        case "Enter":
          e.preventDefault();
          if (selectedIndex >= 0) {
            selectItem(filteredData[selectedIndex]);
          }
          break;

        case "Escape":
          input.blur();
          selectedIndex = -1;
          updateSelection();
          break;
      }
    });

    function updateDropdown(items, searchTerm) {
      dropdown.innerHTML = "";

      if (items.length === 0) {
        var noResults = document.createElement("div");
        noResults.className = "dropdown-item no-results";
        noResults.textContent = searchTerm
          ? "No results found"
          : "No text snippets available";
        dropdown.appendChild(noResults);
      } else {
        items.forEach(function (item, index) {
          var formattedItem = formatItemDisplay(item);

          var div = document.createElement("div");
          div.className = "dropdown-item";
          div.title = formattedItem.title; // Show key on hover

          // Set display text
          var displayText = formattedItem.displayText;

          // Highlight search term in display text
          if (searchTerm) {
            var regex = new RegExp("(" + escapeRegex(searchTerm) + ")", "gi");
            displayText = displayText.replace(regex, "<strong>$1</strong>");
          }

          div.innerHTML = displayText;

          // Store the original item for selection
          div.setAttribute("data-item-index", index);

          div.addEventListener("dblclick", function () {
            selectItem(item);
          });

          dropdown.appendChild(div);
        });
      }
    }

    function updateSelection() {
      var items = dropdown.getElementsByClassName("dropdown-item");
      for (var i = 0; i < items.length; i++) {
        items[i].classList.remove("selected");
      }

      if (selectedIndex >= 0 && items[selectedIndex]) {
        items[selectedIndex].classList.add("selected");
        items[selectedIndex].scrollIntoView({ block: "nearest" });
      }
    }

    function selectItem(item) {
      var formattedItem = formatItemDisplay(item);
      input.value = formattedItem.key;

      // Find index in original array and insert
      var originalIndex = textDataList.indexOf(item);
      if (originalIndex >= 0) {
        insertTextByIndex(originalIndex);
      }
    }

    function escapeRegex(string) {
      return string.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    }

    // Make updateDropdown available globally for refresh
    window.updateDropdownContent = updateDropdown;
  }

  // Setup postMessage listener for cross-iframe communication
  function setupPostMessageListener() {
    window.addEventListener("message", function (event) {
      try {
        var data = event.data;
        if (data) {
          switch (data.type) {
            case "SET_TEXT_DATA":
              if (Array.isArray(data.payload)) {
                textDataList = data.payload;

                // Update UI
                populateTextSelector();

                // Send confirmation back to parent
                if (event.source) {
                  event.source.postMessage(
                    {
                      type: "TEXT_DATA_SET_SUCCESS",
                      messageId: data.messageId, // Include messageId for tracking
                      payload: {
                        count: textDataList.length,
                        message: "Text data set successfully",
                      },
                    },
                    event.origin
                  );
                }
              } else {
                console.error("Invalid data format. Expected array.");
              }
              break;
            case "GET_TEXT_DATA":
              if (event.source) {
                event.source.postMessage(
                  {
                    type: "TEXT_DATA_RESPONSE",
                    messageId: data.messageId, // Include messageId for tracking
                    payload: textDataList,
                  },
                  event.origin
                );
              }
              break;
            case "GET_ALL_KEYS":
              getAllKeysAndNotifyClient(
                event.source,
                event.origin,
                data.messageId
              );
              break;
            case "TEST":
              test(event.source, event.origin, data.messageId);
              break;
            default:
              console.warn("Unknown message type:", data.type);
              break;
          }
        }
      } catch (error) {
        console.error("Error processing postMessage:", error);
      }
    });
  }

  // Notify parent window that plugin is ready and request initial data
  function notifyPluginReady() {
    var readyMessage = {
      type: "PLUGIN_READY",
      payload: {
        message: "Template plugin is ready to receive text data",
        pluginId: "asc.{5F2A8B4C-9D3E-4A7B-8C1F-2E9A7B4C5D6F}",
        timestamp: new Date().toISOString(),
      },
    };

    // Function to send message with retry mechanism
    function sendReadyMessage() {
      try {
        // Send to parent.parent (the actual client window)
        if (
          window.parent &&
          window.parent.parent &&
          window.parent.parent !== window.parent
        ) {
          window.parent.parent.postMessage(readyMessage, "*");
        } else {
          // Fallback to direct parent
          if (window.parent && window.parent !== window) {
            window.parent.postMessage(readyMessage, "*");
          }
        }
      } catch (error) {
        console.error("Error sending plugin ready notification:", error);
      }
    }

    // Send immediately
    sendReadyMessage();
  }

  // Custom method to receive text data from client
  window.Asc.plugin.setTextData = function (data) {
    if (Array.isArray(data)) {
      textDataList = data;
    } else {
      console.error("Invalid data format. Expected array.");
    }
  };

  // Method to get current text data (for debugging)
  window.Asc.plugin.getTextData = function () {
    return textDataList;
  };

  // Handle button clicks (Insert/Cancel from panel)
  window.Asc.plugin.button = function button(id) {
    if (id === 0) {
      // Insert button clicked - insert selected text
      insertSelectedText();
      // Don't close panel - keep it open for multiple insertions
    } else {
      // Cancel button clicked - close panel

      this.executeCommand("close", "");
    }
  };

  // Add context menu items when context menu shows
  window.Asc.plugin.event_onContextMenuShow = function (options) {
    // Add remove-key context menu item
    var items = [
      {
        id: "remove-key",
        text: "Remove Key",
      },
    ];

    this.executeMethod("AddContextMenuItem", [
      {
        guid: this.guid,
        items: items,
      },
    ]);
  };

  // Handle static context menu item clicks
  window.Asc.plugin.event_onContextMenuClick = function (id) {
    // Handle remove-key item
    if (id === "remove-key") {
      removeKeyAtCursor();
      return;
    }
  };

  window.Asc.plugin.event_onTargetPositionChanged = function () {};

  // Function to remove key at cursor position
  function removeKeyAtCursor() {
    Asc.scope.textDataList = textDataList;
    try {
      window.Asc.plugin.callCommand(
        function () {
          var oDocument = Api.GetDocument();
          if (!oDocument) {
            return false;
          }

          try {
            // Get current run
            let oRun = oDocument.GetCurrentRun();
            if (!oRun) return false;
            let cell = oRun.GetParentParagraph()?.GetParentTableCell();
            let oRunText = oRun.GetText();
            if (
              cell &&
              Asc.scope.textDataList.some(
                (data) => oRunText.includes(data.key) && data.type === 2
              )
            ) {
              let row = cell.GetParentRow();
              if (row) {
                let columnsCount = row.GetCellsCount();
                for (let c = 0; c < columnsCount; c++) {
                  let allDoEl = row.GetCell(c)?.GetContent()?.GetContent();
                  if (allDoEl) {
                    for (let el = 0; el < allDoEl.length; el++) {
                      let para = allDoEl[el];
                      if (para && para.GetClassType() === "paragraph") {
                        let paraRunsCount = para.GetElementsCount();
                        for (let r = 0; r < paraRunsCount; r++) {
                          let run = para.GetElement(r);
                          if (run && run.GetClassType() === "run") {
                            run.ClearContent();
                          }
                        }
                      }
                    }
                  }
                }
                return true;
              }
            }
            return oRun.RemoveAllElements();
          } catch (e) {
            console.error("Error in removeKeyAtCursor callCommand:", e);
            return false;
          }
        },
        false,
        false,
        function (result) {
          if (result) {
            console.log("Key removed at cursor");
          } else {
            console.log("No key found at cursor to remove");
          }
        }
      );
    } catch (error) {
      console.error("Error in removeKeyAtCursor:", error);
    }
  }

  // Helper function to get type icon text
  function getTypeIcon(type) {
    switch (type) {
      case 0: // Single
        return "📝"; // Text icon
      case 1: // MultiLineText
        return "📄"; // Document icon
      case 2: // Table
        return "📊"; // Table icon
      default:
        return "📝"; // Default text icon
    }
  }

  function getTitle(item) {
    let title = item.key;
    switch (item.type) {
      case 0: // Single
        title += " (Single)";
        break;
      case 2: // Table
        title += " (Table)";
        break;
    }
    switch (item.dataInsertType) {
      case 0: // Text
        title += " (Text)";
        break;
      case 1: // Image
        title += " (Image)";
        break;
    }
    return title;
  }

  // Helper function to get data insert type icon text
  function getDataInsertTypeIcon(dataInsertType) {
    switch (dataInsertType) {
      case 1: // Image
        return "🖼️"; // Image icon
      default:
        return ""; // Default text icon
    }
  }

  // Helper function to format item display in dropdown
  function formatItemDisplay(item) {
    // Handle both string items (legacy) and object items (IKeyTemplate)
    if (typeof item === "string") {
      return {
        displayText: item,
        searchText: item,
        key: item,
      };
    }

    // Handle IKeyTemplate object
    if (item && typeof item === "object" && item.key) {
      var typeIcon = getTypeIcon(item.type);
      var dataInsertIcon = getDataInsertTypeIcon(item.dataInsertType);

      // Format: [TypeIcon] [DataInsertIcon] Key - Description
      var displayText =
        typeIcon + " " + dataInsertIcon + " " + (item.description || item.key);

      // For search, include key and description
      var searchText =
        item.key + (item.description ? " " + item.description : "");

      return {
        displayText: displayText,
        searchText: searchText,
        key: item.key,
        item: item,
        title: getTitle(item), // For tooltip
      };
    }

    // Fallback for unknown format
    return {
      displayText: String(item),
      searchText: String(item),
      key: String(item),
    };
  }

  // Populate text selector with available data
  function populateTextSelector() {
    var selector = document.getElementById("textSelector");
    var statusMessage = document.getElementById("statusMessage");

    if (!selector) {
      console.error("Text selector not found");
      return;
    }

    // Clear input value
    selector.value = "";

    if (!textDataList || textDataList.length === 0) {
      // Show empty state
      statusMessage.textContent =
        "No text snippets available. Use postMessage to set data.";
      statusMessage.className = "status empty";
      selector.placeholder = "No text snippets available";
      return;
    }

    // Update status
    statusMessage.textContent =
      textDataList.length +
      " text snippets available. Type to search or click to browse.";
    statusMessage.className = "status loaded";

    // Refresh autocomplete dropdown if it exists
    var dropdown = document.getElementById("dropdownList");
    if (dropdown && textDataList.length > 0) {
      // Trigger setupAutocomplete to refresh the dropdown
      setupAutocomplete();
    }
  }

  // Function to insert selected text (from autocomplete)
  function insertSelectedText() {
    var selector = document.getElementById("textSelector");
    if (!selector) {
      console.error("Text selector not found");
      return false;
    }

    var inputValue = selector.value.trim();
    if (!inputValue) {
      console.log("Please enter or select a text snippet");
      return false;
    }

    // Find exact match in textDataList by key
    var exactIndex = -1;
    for (var i = 0; i < textDataList.length; i++) {
      var formattedItem = formatItemDisplay(textDataList[i]);
      if (formattedItem.key === inputValue) {
        exactIndex = i;
        break;
      }
    }

    if (exactIndex >= 0) {
      return insertTextByIndex(exactIndex);
    }

    // If no exact match, try to find partial match
    var partialMatches = [];
    for (var i = 0; i < textDataList.length; i++) {
      var formattedItem = formatItemDisplay(textDataList[i]);
      if (
        formattedItem.searchText
          .toLowerCase()
          .includes(inputValue.toLowerCase())
      ) {
        partialMatches.push(textDataList[i]);
      }
    }

    if (partialMatches.length === 1) {
      var matchIndex = textDataList.indexOf(partialMatches[0]);
      return insertTextByIndex(matchIndex);
    } else if (partialMatches.length > 1) {
      console.log(
        "Multiple matches found. Please be more specific or select from dropdown."
      );
      return false;
    } else {
      console.log("No matching text snippet found.");
      return false;
    }
  }

  // Helper function to insert text with context detection and insertion in one go
  function insertTextWithContext(templateKey, selectedItem) {
    Asc.scope.templateKey = templateKey;
    Asc.scope.selectedItem = selectedItem;
    try {
      window.Asc.plugin.callCommand(
        function () {
          let templateKey = Asc.scope.templateKey;
          let selectedItem = Asc.scope.selectedItem;
          let fontFamily = "Times New Roman"; // Default font
          let fontSize = 12; // Default size

          try {
            let oDocument = Api.GetDocument();

            if (!oDocument) {
              console.error("❌ Could not get document");
              return false;
            }

            // Get the current paragraph where cursor is located
            let oParagraph = oDocument.GetCurrentParagraph();

            if (!oParagraph) {
              // Fallback: Insert simple template
              let oRun = Api.CreateRun();
              oRun.AddText("{{" + templateKey + "}}");
              oDocument.InsertContent([oRun]);
              return false;
            }
            let originParaFontFamily = oParagraph.GetTextPr().GetFontFamily();
            let originParaFontSize = oParagraph.GetTextPr().GetFontSize();
            if (originParaFontFamily) fontFamily = originParaFontFamily;
            if (originParaFontSize) fontSize = originParaFontSize;

            // Check class type to ensure it's a paragraph
            let classType = oParagraph.GetClassType();
            if (classType === "paragraph") {
              // Check if paragraph is in a table
              let oTable = oParagraph.GetParentTable();
              if (oTable && oTable !== null && selectedItem.type === 2) {
                // We're in a table and item is a table template - insert full row
                let oTableCell = oParagraph.GetParentTableCell();
                let columnsCount = 0; // Default
                if (oTableCell) {
                  try {
                    // Get current row
                    let oRow = oTableCell.GetParentRow();
                    if (oRow) {
                      // Get columns count from current row
                      if (oRow.GetCellsCount) {
                        columnsCount = oRow.GetCellsCount();
                      }
                      // Clear and fill all cells in current row
                      for (let c = 0; c < columnsCount; c++) {
                        try {
                          let oCell = oRow.GetCell(c);
                          if (oCell) {
                            // Clear cell content
                            if (c == 0) {
                              // Hack :D
                              fontFamily = oCell
                                .GetContent()
                                .GetElement(0)
                                .GetTextPr()
                                .GetFontFamily();
                              fontSize = oCell
                                .GetContent()
                                .GetElement(0)
                                .GetTextPr()
                                .GetFontSize();
                              oCell.Clear();
                            }

                            // Get default paragraph or create new
                            let defaultPara = oCell.GetContent().GetElement(0);
                            if (
                              !defaultPara ||
                              defaultPara.GetClassType() !== "paragraph"
                            ) {
                              defaultPara = Api.CreateParagraph();
                              oCell.AddElement(0, defaultPara);
                            }
                            // Add new paragraph with template
                            let templateText =
                              "{{" + templateKey + ":c" + (c + 1) + "}}";
                            let oRun = Api.CreateRun();
                            oRun.AddText(templateText);
                            defaultPara.AddElement(oRun);
                            defaultPara.SetFontFamily(fontFamily);
                            defaultPara.SetFontSize(fontSize);
                          }
                        } catch (cellError) {}
                      }

                      return true;
                    }
                  } catch (e) {}
                }

                // Fallback for table context
                let oRun = Api.CreateRun();
                oRun.AddText("{{" + templateKey + "}}");
                oParagraph.AddElement(oRun);
                oParagraph.SetFontFamily(fontFamily);
                oParagraph.SetFontSize(fontSize);
                return true;
              } else if (oTable && oTable !== null) {
                // We're in a table but item is not a table template - insert in current cell

                let oRun = Api.CreateRun();
                oRun.AddText("{{" + templateKey + "}}");
                oParagraph.AddElement(oRun);
                oParagraph.SetFontFamily(fontFamily);
                oParagraph.SetFontSize(fontSize);
                return true;
              } else if (selectedItem.type === 2) {
                // Table template outside of table - insert with table indicator

                let oRun = Api.CreateRun();
                oRun.AddText("{{" + templateKey + "}} [TABLE]");
                oParagraph.AddElement(oRun);
                oParagraph.SetFontFamily(fontFamily);
                oParagraph.SetFontSize(fontSize);
                return true;
              } else {
                // Regular text insertion outside of table

                let oRun = Api.CreateRun();
                oRun.AddText("{{" + templateKey + "}}");
                oParagraph.AddElement(oRun);
                oParagraph.SetFontFamily(fontFamily);
                oParagraph.SetFontSize(fontSize);
                return true;
              }
            } else {
              // Current element is not a paragraph - fallback

              let oRun = Api.CreateRun();
              oRun.AddText("{{" + templateKey + "}}");
              oDocument.InsertContent([oRun]);
              return true;
            }
          } catch (innerError) {
            console.error("❌ Inner callCommand error:", innerError);

            // Final fallback
            try {
              let oRun = Api.CreateRun();
              oRun.AddText("{{" + templateKey + "}}");
              oDocument.InsertContent([oRun]);
              return true;
            } catch (fallbackError) {
              console.error(
                "❌ Even fallback insertion failed:",
                fallbackError
              );
              return false;
            }
          }
        },
        false,
        false,
        function (result) {
          if (result) {
          } else {
            // Final fallback using executeMethod
            try {
              window.Asc.plugin.executeMethod("PasteText", [
                `{{${templateKey}}}`,
              ]);
            } catch (fallbackError) {
              console.error(
                "Even executeMethod fallback failed:",
                fallbackError
              );
            }
          }
        },
        [templateKey, selectedItem]
      );
    } catch (error) {
      console.error("Error in insertTextWithContext:", error);

      // Final fallback
      try {
        window.Asc.plugin.executeMethod("PasteText", [`{{${templateKey}}}`]);
      } catch (fallbackError) {
        console.error("Even final fallback failed:", fallbackError);
      }
    }
  }

  // Helper function to insert text by index
  function insertTextByIndex(index) {
    if (index < 0 || index >= textDataList.length) {
      console.error("Invalid text index:", index);
      return false;
    }

    var selectedItem = textDataList[index];
    var textToInsert;
    var displayName;

    // Handle both string and IKeyTemplate object
    if (typeof selectedItem === "string") {
      textToInsert = selectedItem;
      displayName = selectedItem;
      // Create object for consistency
      selectedItem = { key: selectedItem, type: 0, dataInsertType: 0 };
    } else if (
      selectedItem &&
      typeof selectedItem === "object" &&
      selectedItem.key
    ) {
      textToInsert = selectedItem.key;
      displayName = selectedItem.key;
    } else {
      console.error("Invalid item format:", selectedItem);
      return false;
    }

    // Use the new combined function for context detection and insertion
    insertTextWithContext(textToInsert, selectedItem);

    // Update UI immediately (don't wait for async completion)
    console.log(
      "Text inserted: " +
        (displayName.length > 50
          ? displayName.substring(0, 50) + "..."
          : displayName)
    );

    // Clear input for next insertion
    var selector = document.getElementById("textSelector");
    if (selector) {
      selector.value = "";

      // Refresh dropdown to show all items again
      var dropdown = document.getElementById("dropdownList");
      if (dropdown) {
        var filteredData = textDataList.slice(); // Copy array
        updateDropdownContent(filteredData, "");
      }
    }

    return true;
  }

  // Function to get all keys that exist in document and send message to client
  function getAllKeysAndNotifyClient(eventSource, eventOrigin, messageId) {
    try {
      // Use callCommand to scan document for existing keys
      window.Asc.plugin.callCommand(
        function () {
          var foundKeys = [];
          var keySet = new Set(); // For distinct keys
          try {
            var oDocument = Api.GetDocument();
            if (!oDocument) {
              return { foundKeys: [], error: "Could not get document" };
            }

            // Helper function to extract keys from text
            function extractKeysFromText(text) {
              if (!text) return [];
              var keyRegex = /\{\{([^}]+)\}\}/g;
              var matches = [];
              var match;

              while ((match = keyRegex.exec(text)) !== null) {
                var keyName = match[1].trim();
                // Remove column indicators like ":c1", ":c2" for table keys
                keyName = keyName.replace(/:\s*c\d+/g, "");
                if (keyName) {
                  matches.push(keyName);
                }
              }
              return matches;
            }

            // Helper function to scan paragraph for keys
            function scanParagraph(oParagraph) {
              if (!oParagraph || oParagraph.GetClassType() !== "paragraph")
                return;

              try {
                var elementsCount = oParagraph.GetElementsCount();
                for (var i = 0; i < elementsCount; i++) {
                  var element = oParagraph.GetElement(i);
                  if (element && element.GetClassType() === "run") {
                    var runText = element.GetText();
                    if (runText) {
                      var keys = extractKeysFromText(runText);
                      keys.forEach(function (key) {
                        if (!keySet.has(key)) {
                          keySet.add(key);
                          foundKeys.push(key);
                        }
                      });
                    }
                  }
                }
              } catch (e) {
                console.error("Error scanning paragraph:", e);
              }
            }

            // Helper function to scan table
            function scanTable(oTable) {
              if (!oTable) return;

              try {
                var rowsCount = oTable.GetRowsCount();
                for (var r = 0; r < rowsCount; r++) {
                  var oRow = oTable.GetRow(r);
                  if (oRow) {
                    var cellsCount = oRow.GetCellsCount();
                    for (var c = 0; c < cellsCount; c++) {
                      var oCell = oRow.GetCell(c);
                      if (oCell) {
                        var cellContent = oCell.GetContent();
                        if (cellContent) {
                          var elementsCount = cellContent.GetElementsCount();
                          for (var e = 0; e < elementsCount; e++) {
                            var element = cellContent.GetElement(e);
                            if (
                              element &&
                              element.GetClassType() === "paragraph"
                            ) {
                              scanParagraph(element);
                            } else if (
                              element &&
                              element.GetClassType() === "table"
                            ) {
                              scanTable(element); // Nested tables
                            }
                          }
                        }
                      }
                    }
                  }
                }
              } catch (e) {
                console.error("Error scanning table:", e);
              }
            }

            // Scan entire document
            var elementsCount = oDocument.GetElementsCount();
            for (var i = 0; i < elementsCount; i++) {
              var element = oDocument.GetElement(i);
              if (!element) continue;

              var classType = element.GetClassType();
              if (classType === "paragraph") {
                scanParagraph(element);
              } else if (classType === "table") {
                scanTable(element);
              }
            }

            return { foundKeys: foundKeys, error: null };
          } catch (error) {
            console.error("Error scanning document:", error);
            return { foundKeys: [], error: error.message };
          }
        },
        false,
        false,
        function (result) {
          try {
            if (result && result.foundKeys) {
              var foundKeys = result.foundKeys;

              // Match found keys with textDataList to get full info
              var keysWithInfo = [];

              foundKeys.forEach(function (keyName) {
                // Find matching item in textDataList
                var matchedItem = null;
                for (var i = 0; i < textDataList.length; i++) {
                  var item = textDataList[i];
                  var itemKey = "";

                  if (typeof item === "string") {
                    itemKey = item;
                  } else if (item && typeof item === "object" && item.key) {
                    itemKey = item.key;
                  }

                  if (itemKey === keyName) {
                    matchedItem = item;
                    break;
                  }
                }

                // Add to result with info
                if (matchedItem) {
                  if (typeof matchedItem === "string") {
                    keysWithInfo.push({
                      key: matchedItem,
                      type: 0,
                      dataInsertType: 0,
                      description: "",
                      foundInDocument: true,
                    });
                  } else {
                    keysWithInfo.push({
                      key: matchedItem.key,
                      type: matchedItem.type || 0,
                      dataInsertType: matchedItem.dataInsertType || 0,
                      description: matchedItem.description || "",
                      foundInDocument: true,
                    });
                  }
                } else {
                  // Key found in document but not in textDataList
                  keysWithInfo.push({
                    key: keyName,
                    type: 0,
                    dataInsertType: 0,
                    description: "Found in document",
                    foundInDocument: true,
                  });
                }
              });

              // Send success response
              var messageData = {
                type: "ALL_KEYS_RESPONSE",
                messageId: messageId,
                payload: {
                  message: "Keys found in document retrieved successfully",
                  timestamp: new Date().toISOString(),
                  totalKeys: keysWithInfo.length,
                  keys: keysWithInfo,
                },
              };

              sendResponse(messageData);
            } else {
              // No keys found or error
              var errorMsg =
                result && result.error
                  ? result.error
                  : "No keys found in document";
              sendErrorResponse(errorMsg);
            }
          } catch (callbackError) {
            console.error("Error in callCommand callback:", callbackError);
            sendErrorResponse(
              "Error processing document scan results: " + callbackError.message
            );
          }
        }
      );

      // Helper function to send response
      function sendResponse(messageData) {
        if (eventSource) {
          eventSource.postMessage(messageData, eventOrigin);
        } else {
          if (
            window.parent &&
            window.parent.parent &&
            window.parent.parent !== window.parent
          ) {
            window.parent.parent.postMessage(messageData, "*");
          } else if (window.parent && window.parent !== window) {
            window.parent.postMessage(messageData, "*");
          }
        }
      }

      // Helper function to send error response
      function sendErrorResponse(errorMsg) {
        var errorMessage = {
          type: "ALL_KEYS_ERROR",
          messageId: messageId,
          payload: {
            message: "Error getting keys from document",
            error: errorMsg,
            timestamp: new Date().toISOString(),
          },
        };
        sendResponse(errorMessage);
      }
    } catch (error) {
      console.error("Error in getAllKeysAndNotifyClient:", error);

      // Send error message to client
      var errorMessage = {
        type: "ALL_KEYS_ERROR",
        messageId: messageId,
        payload: {
          message: "Error scanning document for keys",
          error: error.message,
          timestamp: new Date().toISOString(),
        },
      };

      try {
        if (eventSource) {
          eventSource.postMessage(errorMessage, eventOrigin);
        } else {
          if (
            window.parent &&
            window.parent.parent &&
            window.parent.parent !== window.parent
          ) {
            window.parent.parent.postMessage(errorMessage, "*");
          } else if (window.parent && window.parent !== window) {
            window.parent.postMessage(errorMessage, "*");
          }
        }
      } catch (sendError) {
        console.error("Failed to send error message:", sendError);
      }
    }
  }

  function test(eventSource, eventOrigin, messageId) {
    try {
      debugger;
      // Use callCommand to scan document for existing keys
      window.Asc.plugin.callCommand(
        function () {
          debugger;
          const keyPairValue = {
            TenGoiThau: "Gói thầu ODA",
            MaDuAn: "TQH",
            TenDuAn: "Đường nối ",
            DanhSachHangMucNghiemThu: [
              {
                values: {
                  value: "1",
                },
                c1: {
                  value: "A",
                },
                c2: {
                  value: "PHẦN NỀN",
                },
                c3: {
                  value: "",
                },
                c4: {
                  value: "",
                },
                c5: {
                  value: "",
                },
                c6: {
                  value: "",
                },
                c7: {
                  value: "",
                },
                c8: {
                  value: "",
                },
                c9: {
                  value: "",
                },
                c10: {
                  value: "",
                },
                c11: {
                  value: "",
                },
                c12: {
                  value: "",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "2",
                },
                c1: {
                  value: "1",
                },
                c2: {
                  value: "Chặt hạ cây đường kính D < 20cm",
                },
                c3: {
                  value: "cây",
                },
                c4: {
                  value: "112",
                },
                c5: {
                  value: "50",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "50",
                },
                c8: {
                  value: "192.752",
                },
                c9: {
                  value: "21.588.224",
                },
                c10: {
                  value: "9.637.600",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "9.637.600",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "3",
                },
                c1: {
                  value: "2",
                },
                c2: {
                  value: "Chặt hạ cây đường kính D < 30cm",
                },
                c3: {
                  value: "cây",
                },
                c4: {
                  value: "29",
                },
                c5: {
                  value: "20",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "20",
                },
                c8: {
                  value: "99.847",
                },
                c9: {
                  value: "2.895.563",
                },
                c10: {
                  value: "1.996.940",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "1.996.940",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "4",
                },
                c1: {
                  value: "3",
                },
                c2: {
                  value: "Đào gốc cây đường kính D < 20cm",
                },
                c3: {
                  value: "gốc",
                },
                c4: {
                  value: "135",
                },
                c5: {
                  value: "50",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "50",
                },
                c8: {
                  value: "387.315",
                },
                c9: {
                  value: "52.287.525",
                },
                c10: {
                  value: "19.365.750",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "19.365.750",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "5",
                },
                c1: {
                  value: "4",
                },
                c2: {
                  value: "Đào gốc cây đường kính D < 30cm",
                },
                c3: {
                  value: "gốc",
                },
                c4: {
                  value: "49",
                },
                c5: {
                  value: "10",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "10",
                },
                c8: {
                  value: "260.043",
                },
                c9: {
                  value: "12.742.107",
                },
                c10: {
                  value: "2.600.430",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "2.600.430",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "6",
                },
                c1: {
                  value: "5",
                },
                c2: {
                  value: "Đào gốc cây đường kính D <60cm",
                },
                c3: {
                  value: "gốc",
                },
                c4: {
                  value: "22",
                },
                c5: {
                  value: "10",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "10",
                },
                c8: {
                  value: "1.022.450",
                },
                c9: {
                  value: "22.493.900",
                },
                c10: {
                  value: "10.224.500",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "10.224.500",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "7",
                },
                c1: {
                  value: "6",
                },
                c2: {
                  value: "Đào bụi tre, tầm vông đường kính D > 80cm",
                },
                c3: {
                  value: "bụi",
                },
                c4: {
                  value: "2",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "344.832",
                },
                c9: {
                  value: "689.664",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "8",
                },
                c1: {
                  value: "7",
                },
                c2: {
                  value:
                    "Đào vét đất cấp 1 nền đường, đổ lên phương tiện vận chuyển (kể cả vận chuyển đổ ra khỏi công trường)",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "164,628",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "75.090.617",
                },
                c9: {
                  value: "12.362.018.095",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "9",
                },
                c1: {
                  value: "8",
                },
                c2: {
                  value:
                    "Đào vét đất cấp 2 nền đường, đổ lên phương tiện vận chuyển (kể cả vận chuyển đổ ra khỏi công trường)",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "223,508",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "100.475.339",
                },
                c9: {
                  value: "22.457.042.069",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "10",
                },
                c1: {
                  value: "9",
                },
                c2: {
                  value:
                    "Đào vét đất cấp 3 nền đường, đổ lên phương tiện vận chuyển (kể cả vận chuyển đổ ra khỏi công trường)",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "44,011",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "14.178.466",
                },
                c9: {
                  value: "624.008.467",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "11",
                },
                c1: {
                  value: "10",
                },
                c2: {
                  value:
                    "Đào vét đất cấp 4 nền đường, đổ lên phương tiện vận chuyển (kể cả vận chuyển đổ ra khỏi công trường)",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "20,594",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "7.547.110",
                },
                c9: {
                  value: "155.425.183",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "12",
                },
                c1: {
                  value: "11",
                },
                c2: {
                  value: "Đắp nền đường đất C3, K=0,95",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "55,033",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "6.925.522",
                },
                c9: {
                  value: "381.132.252",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "13",
                },
                c1: {
                  value: "12",
                },
                c2: {
                  value: "Đắp nền đường đất C3, K=0,98",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "127,885",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "19.082.330",
                },
                c9: {
                  value: "2.440.343.772",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "14",
                },
                c1: {
                  value: "13",
                },
                c2: {
                  value: "Đào cát nền đường đất C2 (KL tận dụng), K=0,95",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "54,621",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "21.447.802",
                },
                c9: {
                  value: "1.171.500.393",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "15",
                },
                c1: {
                  value: "14",
                },
                c2: {
                  value: "Đào cát nền đường, K=0,95",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "97,772",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "97.603.243",
                },
                c9: {
                  value: "9.542.864.274",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "16",
                },
                c1: {
                  value: "15",
                },
                c2: {
                  value: "Đào cát nền đường, K=0,95 (KL tận dụng)",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "7,115",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "1.217.561",
                },
                c9: {
                  value: "8.662.946",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "17",
                },
                c1: {
                  value: "16",
                },
                c2: {
                  value: "Đóng cọc tràm vào đất cấp 1 L>2,5m",
                },
                c3: {
                  value: "100m",
                },
                c4: {
                  value: "2.101,042",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "188.791.194",
                },
                c9: {
                  value: "396.658.227.824",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "18",
                },
                c1: {
                  value: "17",
                },
                c2: {
                  value: "Lu lèn lại mặt đường cũ đã cày phá",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "105,943",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "10.654.707",
                },
                c9: {
                  value: "1.128.791.623",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "19",
                },
                c1: {
                  value: "18",
                },
                c2: {
                  value: "Đổ bê tồng đá 1x2 mác 200 tấm đạn",
                },
                c3: {
                  value: "m3",
                },
                c4: {
                  value: "0",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "20",
                },
                c1: {
                  value: "19",
                },
                c2: {
                  value: "Gia công cốt thép tấm đạn < D10",
                },
                c3: {
                  value: "tấn",
                },
                c4: {
                  value: "0",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "21",
                },
                c1: {
                  value: "20",
                },
                c2: {
                  value: "Làm ván khuôn đúc mốc quan trắc",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "0",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "22",
                },
                c1: {
                  value: "21",
                },
                c2: {
                  value: "Rải vải địa kỹ thuật làm nền đường, mà, đê, đập",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "104,262",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "15.733.196",
                },
                c9: {
                  value: "1.640.374.481",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "23",
                },
                c1: {
                  value: "22",
                },
                c2: {
                  value: "Lắp đặt ống thoát nước PV D114",
                },
                c3: {
                  value: "100m",
                },
                c4: {
                  value: "0,49",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "633.000",
                },
                c9: {
                  value: "310.170",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "24",
                },
                c1: {
                  value: "23",
                },
                c2: {
                  value: "Lu lèn nền  đường nguyên thổ",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "1,35",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "20.599",
                },
                c9: {
                  value: "27.808",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "25",
                },
                c1: {
                  value: "24",
                },
                c2: {
                  value: "Cung cấp ống thép ren D 20 làm mốc quan trắc lún",
                },
                c3: {
                  value: "m",
                },
                c4: {
                  value: "42",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "42.714",
                },
                c9: {
                  value: "1.793.988",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "26",
                },
                c1: {
                  value: "25",
                },
                c2: {
                  value: "Cung cấp đất C3 (sỏi đỏ)",
                },
                c3: {
                  value: "m3",
                },
                c4: {
                  value: "3.352,2",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "23.874.368",
                },
                c9: {
                  value: "80.031.656.409",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "27",
                },
                c1: {
                  value: "B",
                },
                c2: {
                  value: "PHẦN MẶT ĐƯỜNG",
                },
                c3: {
                  value: "",
                },
                c4: {
                  value: "",
                },
                c5: {
                  value: "",
                },
                c6: {
                  value: "",
                },
                c7: {
                  value: "",
                },
                c8: {
                  value: "",
                },
                c9: {
                  value: "",
                },
                c10: {
                  value: "",
                },
                c11: {
                  value: "",
                },
                c12: {
                  value: "",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "28",
                },
                c1: {
                  value: "1",
                },
                c2: {
                  value: "Làm móng cấp phối đá dăm lớp trên, đường mở",
                },
                c3: {
                  value: "100m3",
                },
                c4: {
                  value: "9,498",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "29",
                },
                c1: {
                  value: "2",
                },
                c2: {
                  value: "Rải thảm mặt đường BTNN hạt trung dày 7cm",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "27,138",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "30",
                },
                c1: {
                  value: "3",
                },
                c2: {
                  value: "Rải thảm mặt đường BTNN hạt mịn dày 5cm",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "27,138",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "31",
                },
                c1: {
                  value: "4",
                },
                c2: {
                  value:
                    "Tưới lớp dính bám mặt đường bừng nhựa pha dầu, lượng nhựa 0,5kg/m2",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "27,138",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "32",
                },
                c1: {
                  value: "5",
                },
                c2: {
                  value:
                    "Tưới lớp dính bám mặt đường bừng nhựa pha dầu, lượng nhựa 1,2kg/m2",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "27,138",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "33",
                },
                c1: {
                  value: "6",
                },
                c2: {
                  value: "Cày xới mặt đường đá dăm hoặc láng nhựa",
                },
                c3: {
                  value: "100m2",
                },
                c4: {
                  value: "0",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "34",
                },
                c1: {
                  value: "7",
                },
                c2: {
                  value: "Gia công lắp đặt trụ biển báo D 80 dài 3m (V/c 10km)",
                },
                c3: {
                  value: "cái",
                },
                c4: {
                  value: "4",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "35",
                },
                c1: {
                  value: "8",
                },
                c2: {
                  value: "SXLĐ biẻn báo tròn D 70cm , bát giác cạnh 25cm",
                },
                c3: {
                  value: "cái",
                },
                c4: {
                  value: "0",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "36",
                },
                c1: {
                  value: "9",
                },
                c2: {
                  value: "Sản xuất lắp đặt biển báo tam giác cạnh 70cm",
                },
                c3: {
                  value: "cái",
                },
                c4: {
                  value: "2",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
              {
                values: {
                  value: "37",
                },
                c1: {
                  value: "10",
                },
                c2: {
                  value: "Sản xuất lắp đặt biển báo tam chữ nhật 60x80cm",
                },
                c3: {
                  value: "cái",
                },
                c4: {
                  value: "2",
                },
                c5: {
                  value: "0",
                },
                c6: {
                  value: "0",
                },
                c7: {
                  value: "0",
                },
                c8: {
                  value: "0",
                },
                c9: {
                  value: "0",
                },
                c10: {
                  value: "0",
                },
                c11: {
                  value: "0",
                },
                c12: {
                  value: "0",
                },
                c13: {
                  value: "",
                },
              },
            ],
            SoDotThanhToan: {
              value: "https://static.onlyoffice.com/assets/docs/samples/img/onlyoffice_logo.png",
              dataInsertType: "Image",
            },
            TenNhaThau: "CÔNG TY CỔ PHẦN CÔNG NGHỆ DP UNITY",
            TenChuDauTu: {
              value: "CÔNG TY CỔ PHẦN IDECO VIỆT NAM",
              style: {
                allCaps: true,
                italic: true,
                fontColor: "#eb4034",
                underline: true,
              },
            },
            ThongTinPhuLucHopDong: "",
            GiaTriHopDong: "10.000.000.000",
            NgayHopDong: "26 tháng 09 năm 2025",
            SoHopDong: "HD002",
            ThuHoiTamUngKyNayNgoaiTe: "0",
            TongSoTienTamUngDeNghiNgoaiTe: "10.000.000",
            GiaiNganKyNayTamUng: "0",
            SoTienDeNghiGiaiNganKyNayBangChuEn:
              "One hundred and thirty-seven million two hundred and fifty-one thousand four hundred and thirty-seven",
            SoTienDeNghiGiaiNganKyNayBangChu: "Không",
            TongLuyKeGiaTriGiaiNgan: "1.137.251.437",
            LuyKeGiaTriGiaiNganThanhToan: "41.633.959",
            LuyKeGiaTriGiaiNganTamUng: "1.095.617.478",
            TongGiaTriGiaiNganKyNay: "0",
            GiaiNganKyNayThanhToan: "0",
            LuyKeGiaTriDenCuoiKy: "43.825.220",
            ThanhToanKhoiLuongHoanThanhKyTruoc: "41.633.959",
            TamUngChuaThuHoiKyTruoc: "1.095.617.478",
            ThuHoiTamUngKyNay: "0",
            TongSoTienTamUngDeNghi: "1.000.000.000",
            NgayBienBanNghiemThu: "",
            BienBanNghiemThuSo: "",
            TyLeGiuLaiBaoHanh: "5",
            TyLeThuHoiTamUng: "",
            GiaTriHopDongNgoaiTe: "500000000",
            NgoaiTeEn: "USD",
            NoiTeEn: "VND",
            SoTienDeNghiGiaiNganKyNayBangChuNgoaiTeEn:
              "Two million six hundred and ninety-three thousand seven hundred and fifty-three",
            SoTienDeNghiGiaiNganKyNayBangChuNgoaiTe: "Không",
            TongLuyKeGiaTriGiaiNganNgoaiTe: "12.693.753",
            LuyKeGiaTriGiaiNganThanhToanNgoaiTe: "775.371",
            LuyKeGiaTriGiaiNganTamUngNgoaiTe: "11.918.382",
            TongGiaTriGiaiNganKyNayNgoaiTe: "0",
            GiaiNganKyNayTamUngNgoaiTe: "0",
            GiaiNganKyNayThanhToanNgoaiTe: "0",
            LuyKeGiaTriDenCuoiKyNgoaiTe: "816.180",
            ThanhToanKhoiLuongHoanThanhKyTruocNgoaiTe: "775.371",
            TamUngChuaThuHoiKyTruocNgoaiTe: "11.918.382",
            TongThanhTienKyNay: "0",
            TongThanhTienLuyKeHienTaiNgoaiTe: "816.180",
            TongThanhTienKyNayNgoaiTe: "0",
            TongThanhTienLuyKeKyTruocNgoaiTe: "816.180",
            TongThanhTienLuyKeHienTai: "43.825.220",
            TongThanhTienLuyKeKyTruoc: "43.825.220",
            TongGiaTriKhoiLuongPhatSinh: "",
            SoTienTamUngBangChu: "Không",
            SoTienTamUngDeNghiPhatSinhBangChu: "Không",
          };
          var oDocument = Api.GetDocument();
          applyStyleSingleKey(oDocument, keyPairValue);
          replaceImageKeysInDocument(oDocument, keyPairValue);
          // First pass: Replace all regular (non-table) keys
          var elementsCount = oDocument.GetElementsCount();
          for (var i = 0; i < elementsCount; i++) {
            var element = oDocument.GetElement(i);
            if (!element) continue;

            var classType = element.GetClassType();
            if (classType === "paragraph") {
              
              //replaceKeysInParagraph(element, null, null, false);
            } else if (classType === "table") {
              // Check if this table contains any table keys that need to be processed
              var tableProcessed = false;

              // Check each key in keyPairValue to see if it's a table key for this table
              for (var keyName in keyPairValue) {
                if (
                  keyPairValue.hasOwnProperty(keyName) &&
                  Array.isArray(keyPairValue[keyName])
                ) {
                  // This is a table key - check if this table contains it
                  var tableHasKey = false;
                  var templateRowIndex = -1;
                  var rowsCount = element.GetRowsCount();

                  for (var r = 0; r < rowsCount && !tableHasKey; r++) {
                    var oRow = element.GetRow(r);
                    if (oRow) {
                      var cellsCount = oRow.GetCellsCount();
                      for (var c = 0; c < cellsCount && !tableHasKey; c++) {
                        var oCell = oRow.GetCell(c);
                        if (oCell) {
                          var cellContent = oCell.GetContent();
                          if (cellContent) {
                            var cellElementsCount =
                              cellContent.GetElementsCount();
                            for (
                              var e = 0;
                              e < cellElementsCount && !tableHasKey;
                              e++
                            ) {
                              var cellElement = cellContent.GetElement(e);
                              if (
                                cellElement &&
                                cellElement.GetClassType() === "paragraph"
                              ) {
                                var paraElementsCount =
                                  cellElement.GetElementsCount();
                                for (
                                  var pe = 0;
                                  pe < paraElementsCount && !tableHasKey;
                                  pe++
                                ) {
                                  var paraElement = cellElement.GetElement(pe);
                                  if (
                                    paraElement &&
                                    paraElement.GetClassType() === "run"
                                  ) {
                                    var runText = paraElement.GetText();
                                    if (
                                      runText &&
                                      runText.includes("{{" + keyName + ":")
                                    ) {
                                      tableHasKey = true;
                                      templateRowIndex = r; // Lưu vị trí row chứa key
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }

                  if (tableHasKey && templateRowIndex !== -1) {
                    processTableWithData(
                      element,
                      keyName,
                      keyPairValue[keyName],
                      templateRowIndex
                    );
                    tableProcessed = true;
                    break; // Process one table key per table
                  }
                }
              }

              // If no table keys were processed, still replace regular keys in table cells
              if (!tableProcessed) {
                var rowsCount = element.GetRowsCount();
                for (var r = 0; r < rowsCount; r++) {
                  var oRow = element.GetRow(r);
                  if (oRow) {
                    var cellsCount = oRow.GetCellsCount();
                    for (var c = 0; c < cellsCount; c++) {
                      var oCell = oRow.GetCell(c);
                      if (oCell) {
                        var cellContent = oCell.GetContent();
                        if (cellContent) {
                          var cellElementsCount =
                            cellContent.GetElementsCount();
                          for (var e = 0; e < cellElementsCount; e++) {
                            var cellElement = cellContent.GetElement(e);
                            if (
                              cellElement &&
                              cellElement.GetClassType() === "paragraph"
                            ) {
                              replaceKeysInParagraph(
                                cellElement,
                                null,
                                null,
                                false
                              );
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
          // Helper function to replace single keys in all document
          function replaceSingleKeysInDocument(oDocument, keyPairValue) {
            if (!oDocument || !keyPairValue) return;
            for (var key in keyPairValue) {
              if (!keyPairValue.hasOwnProperty(key)) continue;
              if (Array.isArray(keyPairValue[key])) continue; // Skip table keys
              if (keyPairValue[key].dataInsertType == "Image") continue; // Skip image keys
              var value = keyPairValue[key];
              if (
                typeof keyPairValue[key] === "object" &&
                keyPairValue[key].value !== undefined
              ) {
                value = keyPairValue[key].value;
              }
              if (typeof value !== "string") continue; // Only process string values
              oDocument.SearchAndReplace({
                searchString: "{{" + key + "}}",
                replaceString: value,
              });
            }
          }
          // Helper function to extract keys from text
          function extractKeysFromText(text) {
            if (!text) return [];
            var keyRegex = /\{\{([^}]+)\}\}/g;
            var matches = [];
            var match;

            while ((match = keyRegex.exec(text)) !== null) {
              var keyName = match[1].trim();
              matches.push({
                fullMatch: match[0], // Full match including {{}}
                keyName: keyName,
                isTableKey: keyName.includes(":"),
              });
            }
            return matches;
          }
          // Helper function to replace image single keys in all document
          function replaceImageKeysInDocument(oDocument, keyPairValue) {
            if (!oDocument || !keyPairValue) return;
            for (let key in keyPairValue) {
              if (!keyPairValue.hasOwnProperty(key)) continue;
              if (Array.isArray(keyPairValue[key])) continue; // Skip table keys
              if (keyPairValue[key].dataInsertType !== "Image") continue; // Only process image keys
              let value = keyPairValue[key].value;
              if (typeof value !== "string") continue; // Only process string values
              let allRange = oDocument.Search("{{" + key + "}}");
              if (allRange && allRange.length > 0) {
                for (let i = 0; i < allRange.length; i++) {
                  const range = allRange[i];
                  if (range) {
                    const newPara = Api.CreateParagraph();
                    const width = keyPairValue[key]?.style?.width || 50; // mm
                    const height = keyPairValue[key]?.style?.height || 50; // mm
                    let img = Api.CreateImage(
                      value,
                      width * 36000,
                      height * 36000
                    );
                    newPara.AddDrawing(img);
                    range.Select();
                    oDocument.InsertContent([newPara]);
                  }
                }
              }
            }
          }
          // Helper function convert hex color to RGB
          function hexToRgb(hex) {
            // Bỏ ký tự '#' nếu có
            hex = hex.replace(/^#/, "");

            // Nếu dạng rút gọn (#RGB), chuyển thành dạng đầy đủ (#RRGGBB)
            if (hex.length === 3) {
              hex = hex
                .split("")
                .map((char) => char + char)
                .join("");
            }

            // Tách thành 3 cặp 2 ký tự
            let bigint = parseInt(hex, 16);
            let r = (bigint >> 16) & 255;
            let g = (bigint >> 8) & 255;
            let b = bigint & 255;

            return { r, g, b };
          }
          // Helper function to apply style for single key replacement
          function applyStyleSingleKey(oDocument, keyPairValue) {
            for (let key in keyPairValue) {
              if (!keyPairValue.hasOwnProperty(key)) continue;
              if (Array.isArray(keyPairValue[key])) continue; // Skip table keys
              const styleKey = keyPairValue[key].style;
              if (styleKey === undefined) continue; // Skip if no style defined
              let allRange = oDocument.Search("{{" + key + "}}");
              if (allRange && allRange.length > 0) {
                for (let r = 0; r < allRange.length; r++) {
                  const range = oDocument.GetRange(
                    allRange[r].GetStartPos(),
                    allRange[r].GetEndPos()
                  );
                  if (range) {
                    if (styleKey.allCaps !== undefined)
                      range.SetSmallCaps(!styleKey.allCaps); // boolean
                    //   if (styleKey.baselineAlignment !== undefined)
                    //     textPr.SetBaselineAlignment(styleKey.baselineAlignment);
                    if (styleKey.bold !== undefined)
                      range.SetBold(styleKey.bold); // boolean
                    if (styleKey.fontColor !== undefined) {
                      let rgb = hexToRgb(styleKey.fontColor);
                      range.SetColor(rgb.r, rgb.g, rgb.b);
                    }
                    if (styleKey.fontFamily !== undefined)
                      range.SetFontFamily(styleKey.fontFamily); // string
                    if (styleKey.fontSize !== undefined)
                      range.SetFontSize(styleKey.fontSize * 2);
                    if (styleKey.italic !== undefined)
                      range.SetItalic(styleKey.italic); // boolean
                    if (styleKey.strikethrough !== undefined)
                      range.SetStrikeout(styleKey.strikethrough); // boolean
                    if (styleKey.underline !== undefined)
                      range.SetUnderline(styleKey.underline); // boolean
                    if (styleKey.vertAlign !== undefined)
                      range.SetVertAlign(styleKey.vertAlign); // baseline, superscript, subscript
                    oDocument.UpdateAllFields();
                  }
                }
              }
            }
          }
          // Helper function specifically for table key replacement
          function getTableKeyValue(baseKey, columnKey, rowData, keyPairValue) {
            // First try to get from current rowData
            if (
              rowData &&
              rowData[columnKey] &&
              rowData[columnKey].value !== undefined
            ) {
              return rowData[columnKey].value;
            }

            // If no rowData or column not found, return original
            return "{{" + baseKey + ":" + columnKey + "}}";
          }
          // Function to clone a source row to destination row with formatting and replace table key values
          function cloneRowWithData(
            sourceRow,
            destinationRow,
            baseKey,
            rowData,
            keyPairValue
          ) {
            if (!sourceRow || !destinationRow || !baseKey || !rowData)
              return false;

            try {
              var sourceCellsCount = sourceRow.GetCellsCount();
              var destCellsCount = destinationRow.GetCellsCount();

              // Clone each cell from source to destination
              for (
                var cellIndex = 0;
                cellIndex < sourceCellsCount;
                cellIndex++
              ) {
                var sourceCell = sourceRow.GetCell(cellIndex);
                var destCell = destinationRow.GetCell(cellIndex);

                if (sourceCell && destCell) {
                  cloneCellWithData(
                    sourceCell,
                    destCell,
                    baseKey,
                    rowData,
                    keyPairValue,
                    cellIndex
                  );
                }
              }

              return true;
            } catch (error) {
              console.error("Error cloning row with data:", error);
              return false;
            }
          }

          // Helper function to clone a cell with formatting and replace content
          function cloneCellWithData(
            sourceCell,
            destinationCell,
            baseKey,
            rowData,
            keyPairValue,
            cellIndex
          ) {
            if (!sourceCell || !destinationCell) return false;

            try {
              // Get source cell content
              var sourceContent = sourceCell.GetContent();
              if (!sourceContent) return false;

              var sourceElementsCount = sourceContent.GetElementsCount();

              // Clone each element in the cell (simplified - just copy text with basic formatting)
              for (
                var elemIndex = 0;
                elemIndex < sourceElementsCount;
                elemIndex++
              ) {
                var sourceElement = sourceContent.GetElement(elemIndex);

                if (
                  sourceElement &&
                  sourceElement.GetClassType() === "paragraph"
                ) {
                  // Create new paragraph and copy content with replacement
                  var newParagraph = Api.CreateParagraph();
                  // Get text from paragraph
                  var sourceText = sourceElement.GetText();
                  var sourceTextPr = sourceElement.GetTextPr();

                  // Replace table and regular keys
                  var replacedText = sourceText;
                  // Replace table keys using the new helper function
                  var tableKeyRegex = new RegExp(
                    "\\{\\{\\s*" + baseKey + "\\s*:\\s*([^}]+)\\s*\\}\\}",
                    "g"
                  );
                  replacedText = replacedText.replace(
                    tableKeyRegex,
                    function (match, matchedColumnKey) {
                      var colKey = matchedColumnKey.trim();
                      return getTableKeyValue(
                        baseKey,
                        colKey,
                        rowData,
                        keyPairValue
                      );
                    }
                  );

                  newParagraph.AddText(replacedText);
                  if (sourceTextPr) newParagraph.SetTextPr(sourceTextPr);
                  destinationCell.AddElement(elemIndex, newParagraph);
                }
              }

              return true;
            } catch (error) {
              console.error("Error cloning cell with data:", error);
              return false;
            }
          }

          // Helper function to duplicate table rows for table data
          function processTableWithData(
            oTable,
            baseKey,
            tableData,
            templateRowIndex
          ) {
            if (
              !oTable ||
              !tableData ||
              !Array.isArray(tableData) ||
              templateRowIndex === undefined
            )
              return false;

            try {
              var templateRow = oTable.GetRow(templateRowIndex);
              if (!templateRow) return false;
              var cellsCount = templateRow.GetCellsCount();

              // Add new rows for each data item (insert below template row)
              for (
                var dataIndex = 0;
                dataIndex < tableData.length;
                dataIndex++
              ) {
                var rowData = tableData[dataIndex];

                // Create new row at position templateRowIndex + 1 + dataIndex (insert below template)

                var markCell = templateRow.GetCell(0); // Use first cell to mark position
                if (!markCell) continue;

                var newRow = oTable.AddRow(markCell, true);
                if (newRow) {
                  // Use the new cloneRowWithData function to clone template row with formatting
                  cloneRowWithData(
                    templateRow,
                    newRow,
                    baseKey,
                    rowData,
                    keyPairValue
                  );
                  markCell = newRow.GetCell(0); // Update reference for next insertion
                }
              }

              // Remove the original template row after adding all new rows
              const firstCellTemplate = templateRow.GetCell(0);
              if (firstCellTemplate) oTable.RemoveRow(firstCellTemplate);

              return true;
            } catch (error) {
              console.error("Error processing table with data:", error);
              return false;
            }
          }
        },
        false,
        false
      );
    } catch (error) {
      console.error("Error in getAllKeysAndNotifyClient:", error);
    }
  }
})(window, undefined);

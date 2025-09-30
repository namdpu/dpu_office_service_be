# Context Menu Modal Dialog - Troubleshooting Guide

## 🎯 Problem Description
Plugin YouTube được khởi động từ toolbar và hiển thị modal dialog, còn plugin Add-Key được khởi động từ context menu nhưng modal không hiển thị.

## 🔍 Root Cause Analysis

### Điểm khác biệt chính:

1. **YouTube Plugin**:
   - Được gọi từ toolbar button
   - OnlyOffice tự động tạo modal window
   - Plugin chỉ cần populate content

2. **Add-Key Plugin**:
   - Được gọi từ context menu click
   - Cần tự trigger modal display
   - OnlyOffice có thể không tự động tạo modal window

## ✅ Solutions Implemented

### 1. Config.json Configuration
```json
{
  "isModal": true,
  "size": [400, 350],
  "buttons": [
    {"text": "Insert", "primary": true},
    {"text": "Cancel", "primary": false}
  ],
  "events": ["onContextMenuShow", "onContextMenuClick"]
}
```

### 2. Plugin Code Structure
```javascript
// Context menu click handler
window.Asc.plugin.event_onContextMenuClick = function(id) {
  if (id === "add-key") {
    showAddKeyDialog();
  }
}

// Modal dialog trigger
function showAddKeyDialog() {
  // Request data if not available
  if (!textDataList || textDataList.length === 0) {
    notifyPluginReady();
  }
  
  // Refresh UI and ensure proper sizing
  populateTextSelector();
  window.Asc.plugin.resizeWindow(400, 350, 0, 0, 0, 0);
}
```

## 🧪 Testing Steps

### Step 1: Verify Plugin Installation
1. Plugin folder exists in OnlyOffice plugins directory
2. config.json has correct JSON syntax
3. All required files present: config.json, index.html, pluginCode.js

### Step 2: Test Context Menu Integration
1. Right-click in OnlyOffice document
2. Verify context menu items appear
3. Check browser console for any errors

### Step 3: Test Modal Dialog
1. Click "Add Key" context menu item
2. Modal should appear with text selector
3. If no modal, check console for errors

### Step 4: Test Communication
1. Use browser console to check postMessage events
2. Verify plugin receives PLUGIN_READY messages
3. Test data setting via SET_TEXT_DATA

## 🐛 Common Issues & Solutions

### Issue 1: Modal không hiển thị
**Symptoms**: Context menu click works, data processing occurs, but no modal window

**Solutions**:
- ✅ Ensure `isModal: true` in config.json
- ✅ Check `size` and `buttons` configuration
- ✅ Verify plugin window is properly initialized
- ✅ Use `resizeWindow()` to ensure visibility

### Issue 2: Context menu items không hiển thị
**Symptoms**: Right-click doesn't show plugin items

**Solutions**:
- Check `events` array includes `"onContextMenuShow"` và `"onContextMenuClick"`
- Verify `executeMethod("AddContextMenuItem")` syntax
- Check plugin GUID matches config.json

### Issue 3: Data không được populate
**Symptoms**: Modal appears but empty or no data

**Solutions**:
- ✅ Implement postMessage communication
- ✅ Add PLUGIN_READY notification
- ✅ Handle SET_TEXT_DATA messages
- ✅ Call `populateTextSelector()` on data update

## 📋 Expected Behavior Flow

```
1. User right-clicks → onContextMenuShow()
2. Plugin adds menu items → AddContextMenuItem()
3. User clicks "Add Key" → onContextMenuClick("add-key")
4. Plugin calls showAddKeyDialog()
5. Plugin sends PLUGIN_READY message
6. Parent responds with SET_TEXT_DATA
7. Plugin populates selector
8. Modal becomes visible with data
9. User selects & inserts text
10. Modal closes after insertion
```

## 🔧 Debug Tools

### Browser Console Commands:
```javascript
// Check plugin status
console.log("Plugin:", window.Asc?.plugin);

// Check data
console.log("Data:", window.Asc?.plugin?.getTextData?.());

// Test communication (from parent window)
window.addEventListener('message', (e) => console.log('Message:', e.data));

// Send test data (from parent to plugin)
pluginFrame.postMessage({
  type: 'SET_TEXT_DATA',
  payload: ['Test 1', 'Test 2', 'Test 3']
}, '*');
```

### Plugin Console Logs:
```javascript
// Expected logs in plugin context:
"work - plugin loaded"
"Add-Key plugin initialized"
"Setting up postMessage listener"
"onContextMenuShow called"
"=== Context Menu Clicked === add-key"
"Showing add key dialog"
"Plugin dialog should now be visible with updated data"
```

## ✨ Final Implementation

The implemented solution:
1. ✅ Maintains modal configuration in config.json
2. ✅ Uses resizeWindow() to ensure modal visibility
3. ✅ Implements proper postMessage communication
4. ✅ Handles context menu integration correctly
5. ✅ Provides fallback data handling

This approach ensures the modal dialog appears when triggered from context menu, similar to how toolbar-triggered modals work in OnlyOffice.
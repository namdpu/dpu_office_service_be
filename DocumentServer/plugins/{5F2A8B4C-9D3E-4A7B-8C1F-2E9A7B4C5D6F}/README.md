# Add-Key Plugin for OnlyOffice DocumentServer



Plugin mở rộng OnlyOffice DocumentServer với tính năng Add-Key thông qua context menu, hỗ trợ auto-sync data flow và postMessage communication cho iframe setup.



## Communication Flow (Auto-sync)Plugin mở rộng OnlyOffice DocumentServer với tính năng Add-Key thông qua context menu, hỗ trợ postMessage communication cho iframe setup.



### 🔄 Automatic Data Sync Process:



1. **Plugin Load** → OnlyOffice plugin loads và gửi `PLUGIN_READY` message## Tính năngPlugin này mở rộng OnlyOffice DocumentServer với các tính năng text manipulation và add-key thông qua context menu.A context menu plugin for ONLYOFFICE DocumentServer that adds useful text manipulation tools.

2. **Client Listen** → Parent window nhận `PLUGIN_READY` và tự động gửi text data

3. **Data Sync** → Plugin nhận data và gửi `TEXT_DATA_SET_SUCCESS` confirmation

4. **Ready to Use** → User có thể right-click → "Add Key" → chọn text

### Context Menu Items:

### 📨 Message Types:

1. **UPPERCASE Text** - Chuyển text được chọn thành chữ hoa

- `PLUGIN_READY` - Plugin đã load xong, sẵn sàng nhận data

- `SET_TEXT_DATA` - Gửi text data array đến plugin  2. **lowercase text** - Chuyển text được chọn thành chữ thường  ## Tính năng## Description

- `TEXT_DATA_SET_SUCCESS` - Confirmation data đã được set thành công

- `GET_TEXT_DATA` - Request current text data3. **Capitalize Text** - Viết hoa chữ cái đầu mỗi từ

- `TEXT_DATA_RESPONSE` - Response với current text data

4. **Word Count** - Đếm số từ, ký tự của text được chọn

## Tính năng

5. **Add Key** - Hiển thị dialog để chèn text từ danh sách có sẵn

### Context Menu Items:

1. **UPPERCASE Text** - Chuyển text được chọn thành chữ hoa### Context Menu Items:This plugin integrates text manipulation tools directly into the context menu of ONLYOFFICE documents. When you select text and right-click, you'll see additional options for transforming the selected text.

2. **lowercase text** - Chuyển text được chọn thành chữ thường  

3. **Capitalize Text** - Viết hoa chữ cái đầu mỗi từ### Add-Key Feature:

4. **Word Count** - Đếm số từ, ký tự của text được chọn

5. **Add Key** - Hiển thị dialog để chèn text từ danh sách có sẵn- Nhận danh sách text snippets từ parent window qua postMessage1. **UPPERCASE Text** - Chuyển text được chọn thành chữ hoa



### Add-Key Feature:- Hiển thị modal dialog với danh sách để lựa chọn

- Plugin tự gửi `PLUGIN_READY` signal khi load xong

- Parent window auto-sync text data khi nhận signal- Insert text được chọn vào vị trí cursor2. **lowercase text** - Chuyển text được chọn thành chữ thường  ## Features

- Modal dialog hiển thị danh sách để lựa chọn

- Insert text được chọn vào vị trí cursor



## Cách sử dụng## Cách sử dụng3. **Capitalize Text** - Viết hoa chữ cái đầu mỗi từ



### 1. Cài đặt Plugin

- Copy thư mục plugin vào `sdkjs-plugins`

- Restart OnlyOffice DocumentServer### 1. Cài đặt Plugin4. **Word Count** - Đếm số từ, ký tự của text được chọn### Text Transformations

- Plugin sẽ tự động load (isSystem: true)

- Copy thư mục plugin vào `sdkjs-plugins`

### 2. Auto-sync Setup (Recommended)

- Restart OnlyOffice DocumentServer5. **Add Key** - Hiển thị dialog để chèn text từ danh sách có sẵn- **UPPERCASE** - Convert selected text to uppercase

```javascript

// Listen for plugin ready signal- Plugin sẽ tự động load (isSystem: true)

window.addEventListener('message', function(event) {

    if (event.data.type === 'PLUGIN_READY') {- **lowercase** - Convert selected text to lowercase  

        console.log('Plugin ready! Auto-sending data...');

        ### 2. Sử dụng Text Manipulation

        // Automatically send your text data

        const textData = [- Chọn text trong document### Add-Key Feature:- **Capitalize** - Capitalize the first letter of each word

            "Hello World!",

            "Lorem ipsum dolor sit amet",- Right-click để mở context menu

            "Best regards,",

            "John Doe"- Chọn function muốn sử dụng- Cho phép set danh sách text snippets từ client- **Reverse Text** - Reverse the order of characters in the selected text

        ];

        

        // Send to OnlyOffice iframe

        const iframe = document.querySelector('#onlyoffice-iframe');### 3. Sử dụng Add-Key Feature (PostMessage Communication)- Hiển thị dialog với danh sách để lựa chọn

        iframe.contentWindow.postMessage({

            type: 'SET_TEXT_DATA',

            payload: textData

        }, '*');Vì OnlyOffice chạy trong iframe, plugin sử dụng postMessage để nhận dữ liệu từ parent window.- Insert text được chọn vào vị trí cursor### Text Analysis

    }

    

    if (event.data.type === 'TEXT_DATA_SET_SUCCESS') {

        console.log(`Successfully synced ${event.data.payload.count} items!`);#### Bước 1: Send Text Data via PostMessage- **Word Count** - Display word count, character count, and character count without spaces

    }

});```javascript

```

// Trong parent window (không phải trong OnlyOffice iframe)## Cách sử dụng

### 3. Manual Data Sync (Alternative)

function sendTextDataToOnlyOffice(textArray) {

```javascript

// Send data manually anytime    const onlyOfficeIframe = document.querySelector('#your-onlyoffice-iframe');## Installation

function sendDataToOnlyOffice(textArray) {

    const iframe = document.querySelector('#onlyoffice-iframe');    

    iframe.contentWindow.postMessage({

        type: 'SET_TEXT_DATA',    const message = {### 1. Cài đặt Plugin

        payload: textArray

    }, '*');        type: 'SET_TEXT_DATA',

}

        payload: textArray- Copy thư mục plugin vào `sdkjs-plugins`1. Copy the plugin folder to your ONLYOFFICE DocumentServer plugins directory

// Usage

sendDataToOnlyOffice([    };

    "Text snippet 1",

    "Text snippet 2",     - Restart OnlyOffice DocumentServer2. Restart ONLYOFFICE DocumentServer

    "Text snippet 3"

]);    onlyOfficeIframe.contentWindow.postMessage(message, '*');

```

}- Plugin sẽ tự động load (isSystem: true)3. The plugin will automatically integrate with the context menu

### 4. Using Add-Key Feature

- Right-click trong OnlyOffice document

- Click "Add Key" từ context menu

- Dialog sẽ hiển thị với danh sách text// Example usage

- Chọn text và click "Insert" hoặc double-click

const textData = [

### 5. Demo Files

- **`auto-sync-demo.html`** - Interactive demo với auto-sync flow    "Hello World!",### 2. Sử dụng Text Manipulation## Usage

- **`demo.html`** - Full demo với OnlyOffice iframe embedded

- **`test-client.html`** - Test client với manual controls    "Lorem ipsum dolor sit amet",



## API Communication    "Best regards,",- Chọn text trong document



### Plugin Ready Notification    "John Doe"

```javascript

// Plugin sends this when loaded];- Right-click để mở context menu1. Open a document in ONLYOFFICE (Word, Cell, or Slide)

{

    type: 'PLUGIN_READY',

    payload: {

        message: 'Add-Key plugin is ready to receive text data',sendTextDataToOnlyOffice(textData);- Chọn function muốn sử dụng2. Select some text in your document

        pluginId: 'asc.{5F2A8B4C-9D3E-4A7B-8C1F-2E9A7B4C5D6F}',

        timestamp: '2025-09-23T10:30:45.123Z'```

    }

}3. Right-click to open the context menu

```

#### Bước 2: Listen for Confirmation

### Send Text Data

```javascript```javascript### 3. Sử dụng Add-Key Feature4. Look for the text manipulation options in the context menu:

// Client sends to plugin

{// Listen for success confirmation

    type: 'SET_TEXT_DATA',

    payload: ['text1', 'text2', 'text3']window.addEventListener('message', function(event) {   - **UPPERCASE** - Converts text to all capital letters

}

```    if (event.data.type === 'TEXT_DATA_SET_SUCCESS') {



### Success Confirmation        console.log(`Successfully set ${event.data.payload.count} text snippets!`);#### Bước 1: Set Text Data   - **lowercase** - Converts text to all lowercase letters

```javascript

// Plugin confirms data received    }

{

    type: 'TEXT_DATA_SET_SUCCESS',});```javascript   - **Capitalize** - Capitalizes the first letter of each word

    payload: {

        count: 3,```

        message: 'Text data set successfully'

    }// Trong browser console hoặc client application   - **Reverse Text** - Reverses the character order

}

```#### Bước 3: Sử dụng Add-Key



### Get Current Data- Right-click trong OnlyOffice documentvar textData = [   - **Word Count** - Shows statistics about the selected text

```javascript

// Client requests current data- Click "Add Key" từ context menu

{

    type: 'GET_TEXT_DATA'- Dialog sẽ hiển thị với danh sách text    "Hello World!",

}

- Chọn text và click "Insert" hoặc double-click

// Plugin responds with current data

{    "Lorem ipsum dolor sit amet",The context menu items will only appear when text is selected. The transformed text will replace the original selection in your document (except for Word Count, which displays information without modifying the text).

    type: 'TEXT_DATA_RESPONSE',

    payload: ['current', 'text', 'data']### 4. Demo Files

}

```- **`test-client.html`** - Test client với postMessage communication    "Best regards,",



## Implementation Details- **`demo.html`** - Full demo với OnlyOffice iframe embedded



### Auto-sync Flow:    "John Doe",## Supported Languages

1. **Plugin init()** → `setupPostMessageListener()` → `notifyPluginReady()`

2. **Client receives** `PLUGIN_READY` → auto-call data sync function## API Communication

3. **Data sync** → plugin stores data và gửi confirmation

4. **User interaction** → context menu "Add Key" → dialog → insert    "Contact: john@example.com"



### Security & Compatibility:### PostMessage API (Recommended for iframe setup)

- ✅ Cross-iframe communication safe

- ✅ Auto-timing với plugin ready signal];- English

- ✅ Error handling & validation

- ✅ Browser compatibility (Chrome, Firefox, Safari, Edge)#### Send Text Data



## File Structure```javascript- Russian (Русский)



```// Send from parent window to OnlyOffice iframe

{5F2A8B4C-9D3E-4A7B-8C1F-2E9A7B4C5D6F}/

├── config.json              # Plugin config (isSystem: true, isModal: true)function sendTextDataToOnlyOffice(textArray) {// Set data vào plugin- French (Français)

├── index.html               # Plugin UI with styles

├── pluginCode.js            # Main logic + postMessage + auto-notify    const iframe = document.querySelector('#onlyoffice-iframe');

├── auto-sync-demo.html      # Interactive demo với auto-sync flow  

├── demo.html                # Full demo với embedded OnlyOffice iframe    window.Asc.plugin.setTextData(textData);- Spanish (Español)  

├── test-client.html         # Test client với manual controls

├── README.md                # Documentation    iframe.contentWindow.postMessage({

└── resources/               # Icon resources

```        type: 'SET_TEXT_DATA',```- Portuguese (Português)



## Advanced Usage        payload: textArray



### Custom Auto-sync Logic    }, '*');- German (Deutsch)

```javascript

let textDataStore = [];}

let pluginReady = false;

```#### Bước 2: Sử dụng Add-Key- Italian (Italiano)

// Update data store

function updateTextData(newData) {

    textDataStore = newData;

    #### Get Text Data  - Right-click trong document

    // Auto-sync if plugin is ready

    if (pluginReady) {```javascript

        syncToOnlyOffice();

    }// Request current data- Click "Add Key" từ context menu## Technical Details

}

iframe.contentWindow.postMessage({

// Sync to OnlyOffice

function syncToOnlyOffice() {    type: 'GET_TEXT_DATA'- Dialog sẽ hiển thị với danh sách text

    const iframe = document.querySelector('#onlyoffice-iframe');

    iframe.contentWindow.postMessage({}, '*');

        type: 'SET_TEXT_DATA',

        payload: textDataStore- Chọn text và click "Insert" hoặc double-click- **Plugin Type**: Context Menu

    }, '*');

}// Listen for response



// Listen for plugin readywindow.addEventListener('message', function(event) {- **Supported Editors**: Word, Cell, Slide

window.addEventListener('message', function(event) {

    if (event.data.type === 'PLUGIN_READY') {    if (event.data.type === 'TEXT_DATA_RESPONSE') {

        pluginReady = true;

        syncToOnlyOffice(); // Auto-sync current data        console.log('Current data:', event.data.payload);### 4. Demo Client- **Requires Selection**: Yes

    }

});    }

```

});Mở file `test-client.html` trong browser để test plugin:- **Modal**: No

### Dynamic Data Updates

```javascript```

// Update data anytime and auto-sync

function addTextSnippet(text) {- Load sample data- **Visual Interface**: Minimal (background processing)

    textDataStore.push(text);

    updateTextData(textDataStore);#### Message Types

}

- `SET_TEXT_DATA` - Set text snippets array- Set custom text data

function removeTextSnippet(index) {

    textDataStore.splice(index, 1);- `GET_TEXT_DATA` - Request current text data  

    updateTextData(textDataStore);

}- `TEXT_DATA_SET_SUCCESS` - Confirmation of successful set- Test plugin connection## Version



// Usage- `TEXT_DATA_RESPONSE` - Response with current data

addTextSnippet("New snippet");

removeTextSnippet(0);

```

### Direct API (If accessible)

## Troubleshooting

```javascript## API Methods1.0.0

### Plugin không gửi PLUGIN_READY

- Check console log cho "Notifying parent window that plugin is ready"// Only works if plugin is directly accessible (not in iframe)

- Verify plugin load successful: "Text Tools Plugin Loaded!" alert

- Check iframe context: plugin phải chạy trong iframe để gửi messagewindow.Asc.plugin.setTextData(textArray);



### Auto-sync không hoạt độngvar currentData = window.Asc.plugin.getTextData();

- Check listener for 'PLUGIN_READY' message trong parent window

- Verify iframe reference chính xác```### setTextData(array)## License

- Check console log cho "Plugin ready notification sent"



### Data sync thất bại

- Check console log cho "Setting text data from postMessage"## Implementation DetailsSet danh sách text snippets cho Add-Key feature.

- Verify data format là array of strings

- Check iframe.contentWindow accessible



### Context menu không hiện Add Key### Communication Flow:Licensed under the Apache License, Version 2.0

- Verify plugin load với isSystem: true

- Check console log cho "onContextMenuShow called"1. **Parent Window** gửi message với type `SET_TEXT_DATA`**Parameters:**

- Restart OnlyOffice DocumentServer

2. **Plugin** nhận message và lưu data vào `textDataList`- `array` - Array of strings

## Browser Compatibility

- Chrome/Chromium ✅3. **Plugin** gửi confirmation với type `TEXT_DATA_SET_SUCCESS`

- Firefox ✅  

- Safari ✅4. **User** right-click → "Add Key" → dialog hiển thị**Example:**

- Edge ✅

- OnlyOffice Desktop/Web versions ✅5. **Plugin** insert selected text vào document```javascript



## Noteswindow.Asc.plugin.setTextData([

- Plugin hoạt động như system plugin (auto-load, không cần user activate)

- Auto-sync flow đảm bảo data sẵn sàng ngay khi plugin load### Security:    "Snippet 1",

- PostMessage communication cross-iframe compatible

- Text data lưu trong memory, reset khi reload page- PostMessage với wildcard origin (`*`) - có thể restrict theo domain    "Snippet 2", 

- Modal dialog responsive và user-friendly

- Hỗ trợ all OnlyOffice editors: Word, Excel, PowerPoint- Data validation trong plugin (check Array.isArray)    "Snippet 3"

- Error handling cho invalid messages]);

```

## File Structure

### getTextData()

```Lấy danh sách text hiện tại.

{5F2A8B4C-9D3E-4A7B-8C1F-2E9A7B4C5D6F}/

├── config.json           # Plugin configuration (isSystem: true, isModal: true)**Returns:** Array of strings

├── index.html            # Plugin UI with styles

├── pluginCode.js         # Main plugin logic + postMessage listener**Example:**

├── test-client.html      # Test client với postMessage functions```javascript

├── demo.html             # Full demo với embedded OnlyOffice iframe  var currentData = window.Asc.plugin.getTextData();

├── README.md             # Documentationconsole.log(currentData);

└── resources/            # Icon resources```

```

## Implementation Details

## Troubleshooting

### Phương án Implementation: Custom Method API

### Plugin không load

- Check console log cho "Text Tools Plugin Loaded!" alert**Ưu điểm:**

- Verify plugin trong OnlyOffice plugin manager  - ✅ API rõ ràng, dễ sử dụng

- Restart OnlyOffice DocumentServer- ✅ Không giới hạn dung lượng

- ✅ Có thể cập nhật bất kỳ lúc nào

### PostMessage không work- ✅ Tương thích tốt với OnlyOffice plugin architecture

- Check console log cho "Received postMessage:" trong OnlyOffice iframe- ✅ Hiệu suất tốt

- Verify iframe có thể access: `iframe.contentWindow`

- Check origin restrictions nếu có**Cách hoạt động:**

1. Client gọi `setTextData()` để set danh sách text

### Context menu không hiện items  2. Plugin lưu data vào biến global `textDataList`

- Check console log cho "onContextMenuShow called"3. Khi click "Add Key", plugin hiển thị dialog với danh sách

- Verify `isSystem: true` trong config.json4. User chọn text và plugin insert vào document

- Check plugin events configuration

### Context Menu Integration

### Add-Key không có data- Plugin sử dụng `event_onContextMenuShow` để add items động

- Send message với type `SET_TEXT_DATA` trước- `isSystem: true` để plugin auto-load

- Check console log: "Setting text data from postMessage"- Modal dialog được tạo động trong HTML

- Verify data format là array of strings

## File Structure

### Dialog không hiển thị

- Check console log cho "Showing add key dialog"```

- Verify text data đã được set qua postMessage{5F2A8B4C-9D3E-4A7B-8C1F-2E9A7B4C5D6F}/

- Check `isModal: true` và `isVisual: true` trong config├── config.json           # Plugin configuration

├── index.html            # Plugin UI with styles

## Example Integration├── pluginCode.js         # Main plugin logic

├── test-client.html      # Demo client for testing

### Basic Integration├── README.md             # Documentation

```html└── resources/            # Icon resources

<!DOCTYPE html>```

<html>

<head>## Troubleshooting

    <title>OnlyOffice with Add-Key Plugin</title>

</head>### Plugin không load

<body>- Check console log cho "Text Tools Plugin Loaded!" alert

    <!-- Your text input -->- Verify plugin trong OnlyOffice plugin manager

    <textarea id="textInput"></textarea>- Restart OnlyOffice DocumentServer

    <button onclick="sendToOnlyOffice()">Set Text Data</button>

    ### Context menu không hiện items

    <!-- OnlyOffice iframe -->- Check console log cho "onContextMenuShow called"

    <iframe id="onlyoffice" src="your-onlyoffice-url"></iframe>- Verify `isSystem: true` trong config.json

    - Check plugin events configuration

    <script>

        function sendToOnlyOffice() {### Add-Key không có data

            const text = document.getElementById('textInput').value;- Call `window.Asc.plugin.setTextData()` trước

            const textArray = text.split('\n').filter(line => line.trim());- Check console log: "setTextData called with:"

            - Verify data format là array of strings

            const iframe = document.getElementById('onlyoffice');

            iframe.contentWindow.postMessage({### Dialog không hiển thị

                type: 'SET_TEXT_DATA',- Check console log cho "Showing add key dialog"

                payload: textArray- Verify text data đã được set

            }, '*');- Check browser console cho errors

        }

        ## Development

        // Listen for confirmations

        window.addEventListener('message', function(event) {### Debug Mode

            if (event.data.type === 'TEXT_DATA_SET_SUCCESS') {Plugin có extensive logging:

                console.log('Data set successfully!');```javascript

            }console.log("setTextData called with:", data);

        });console.log("onContextMenuShow called:", options);

    </script>console.log("Context Menu Clicked:", id);

</body>```

</html>

```### Testing

1. Use `test-client.html` for quick testing

## Browser Compatibility2. Load sample data and test context menu

- Chrome/Chromium ✅3. Check browser console for debug logs

- Firefox ✅  4. Verify text insertion works

- Safari ✅

- Edge ✅## Browser Compatibility

- OnlyOffice Desktop/Web versions ✅- Chrome/Chromium

- Firefox

## Notes- Safari

- Plugin hoạt động như system plugin (auto-load)- Edge

- PostMessage communication cross-iframe compatible- OnlyOffice Desktop/Web versions

- Text data lưu trong memory, sẽ mất khi reload

- Hỗ trợ all OnlyOffice editors: Word, Excel, PowerPoint## Notes

- Modal dialog responsive và user-friendly- Plugin được thiết kế như system plugin (auto-load)
- Modal dialog tương thích với OnlyOffice API
- Text data được lưu trong memory, sẽ mất khi reload
- Hỗ trợ all OnlyOffice editors: Word, Excel, PowerPoint
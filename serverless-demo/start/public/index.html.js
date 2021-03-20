// const LOCAL_BASE_URL = 'http://localhost:7071';

// const app = new Vue({
//     el: '#app',
//     interval: null,
//     data() { 
//         return {
//             stocks: []
//         }
//     },
//     methods: {
//         async update() {
//             try {
//                 const apiUrl = `${LOCAL_BASE_URL}/api/getStocks`;
//                 const response = await axios.get(apiUrl);
//                 console.log('Stocks fetched from ', apiUrl);
//                 app.stocks = response.data;
//             } catch (ex) {
//                 console.error(ex);
//             }
//         },
//         startPoll() {
//             this.interval = setInterval(this.update, 5000);
//         }
//     },
//     created() {
//         this.update();
//         this.startPoll();
//     }
// });
const LOCAL_BASE_URL = 'http://localhost:7071';
const REMOTE_BASE_URL = 'https://fvoice.azurewebsites.net';

const getAPIBaseUrl = () => {
    const isLocal = /localhost/.test(window.location.href);
    return isLocal ? LOCAL_BASE_URL : REMOTE_BASE_URL;
}

const app = new Vue({
    el: '#app',
    data() {
        return {
            stocks: []
        }
    },
    methods: {
        async getStocks() {
            try {
                const apiUrl = `${getAPIBaseUrl()}/api/getStocks`;
                const response = await axios.get(apiUrl);
                app.stocks = response.data;
            } catch (ex) {
                console.error(ex);
            }
        }
    },
    created() {
        this.getStocks();
    }
});

const connect = () => {
    const connection = new signalR.HubConnectionBuilder()
                            .withUrl(`${getAPIBaseUrl()}/api`)
                            .build();

    connection.onclose(()  => {
        console.log('SignalR connection disconnected');
        setTimeout(() => connect(), 2000);
    });

    connection.on('updated', updatedStock => {
        const index = app.stocks.findIndex(s => s.id === updatedStock.id);
        app.stocks.splice(index, 1, updatedStock);
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect();
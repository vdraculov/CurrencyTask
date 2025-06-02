"use strict";

(function () {
    const Converter = {
        state: {
            amount: null,
            currencies: []
        },

        init: function () {
            $("#convert").click(this.handleClick.bind(this));
        },

        handleClick: function () {
            $("#result").hide();
            $("#error").hide();

            this.initState();
            if (!this.state.amount || this.state.currencies.some(c => !c)) {
                $("#error").text("Please enter an amount and select all 3 currencies.").slideDown();
                return;
            }

            $.ajax({
                url: "/Currency/ProcessRates",
                method: "POST",
                data: JSON.stringify(this.state),
                contentType: "application/json",
                success: function (res) {
                    const html = Converter.buildTable(res);
                    $("#result").html(html).fadeIn();
                },
                error: function (xhr) {
                    $("#error").text(xhr.responseText || "An unexpected error occurred").slideDown();
                }
            });
        },

        buildTable: function (data) {
            const currencies = Object.keys(data[0].converted);
            const max = {};
            const min = {};
            currencies.forEach(c => {
                const values = data.map(d => d.converted[c]);
                max[c] = Math.max(...values);
                min[c] = Math.min(...values);
            });

            let html = `<table class="table table-bordered"><thead class="table-dark"><tr><th>Date</th>`;
            currencies.forEach(c => html += `<th>${c}</th>`);
            html += `</tr></thead><tbody>`;

            data.forEach((row, idx) => {
                html += `<tr class="${idx % 2 === 0 ? 'table-light' : ''}"><td>${row.date.split('T')[0]}</td>`;
                currencies.forEach(c => {
                    const rawVal = row.converted[c];
                    const color = rawVal === max[c] ? 'green' : rawVal === min[c] ? 'red' : 'black';
                    html += `<td style="color:${color}">${rawVal.toFixed(2)}</td>`;
                });
                html += `</tr>`;
            });

            html += `</tbody></table>`;
            return html;
        },

        initState: function () {
            this.state.amount = $("#amount").val();
            this.state.currencies = [$("#cur1").val(), $("#cur2").val(), $("#cur3").val()];
        }
    };

    Converter.init();
})();
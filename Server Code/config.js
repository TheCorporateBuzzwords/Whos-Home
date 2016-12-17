module.exports = {
    cryptoConfig: {
        hashBytes: 64,
        saltBytes: 16,
        // more iterations means an attacker has to take longer to brute force an
        // individual password, so larger is better. however, larger also means longer
        // to hash the password. tune so that hashing the password takes about a
        // second
        iterations: 872791,
        digest: 'sha512'
    },   
    connectionInfo: {
        host: "96.41.173.205",
        port: 3306,
        user: "limited",
        password: "$t@ched&&CA$H3D",
        database: "WHOSHOME"
    }
}
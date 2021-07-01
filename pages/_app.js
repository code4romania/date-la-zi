import { useEffect } from 'react'
import { registerMap } from 'echarts'
import '@code4ro/taskforce-fe-components/dist/index.css'
import Head from 'next/head'
import { useRouter } from 'next/router'
import * as gtag from '../utils/gtag'
import GoogleAnalytics from '../components/google-analytics/google-analytics'
import './../styles/index.scss'
import roGeoJson from './../config/roGeo.json'

registerMap('RO', roGeoJson)

function MyApp({ Component, pageProps }) {
  const router = useRouter()
  useEffect(() => {
    const handleRouteChange = (url) => {
      gtag.pageview(url)
    }
    router.events.on('routeChangeComplete', handleRouteChange)
    return () => {
      router.events.off('routeChangeComplete', handleRouteChange)
    }
  }, [router.events])

  return (
    <>
      <Head>
        <title>COVID-19: Date La Zi</title>
        <meta name="viewport" content="initial-scale=1.0, width=device-width" />
        <meta name="theme-color" content="#f5bcd6" />
        <meta
          name="description"
          content="Datelazi.ro prezintă infografii actualizate periodic cu datele furnizate de autoritățile competente."
        />

        <link
          rel="apple-touch-icon"
          sizes="180x180"
          href="/images/favicon/apple-touch-icon.png"
        />
        <link
          rel="icon"
          type="image/png"
          sizes="96x96"
          href="/images/favicon/favicon-96x96.png"
        />
        <link
          rel="icon"
          type="image/png"
          sizes="32x32"
          href="/images/favicon/favicon-32x32.png"
        />
        <link
          rel="icon"
          type="image/png"
          sizes="16x16"
          href="/images/favicon/favicon-16x16.png"
        />
        <link rel="manifest" href="/manifest.json" />
        <link rel="shortcut icon" href="/images/favicon/favicon.ico" />
        <link rel="icon" href="/images/favicon/favicon.ico" />

        <meta
          property="og:image"
          content="https://datelazi.ro/datelazi_banner.jpg"
        />
        <meta property="og:type" content="website" />
        <meta property="og:url" content="https://datelazi.ro" />
        <meta property="og:site_name" content="COVID-19: Date La Zi" />
        <meta property="og:title" content="Date la zi" />
        <meta
          property="og:description"
          content="Datelazi.ro prezintă infografii actualizate periodic cu datele furnizate de autoritățile competente."
        />
      </Head>
      <GoogleAnalytics />
      <Component {...pageProps} />
    </>
  )
}

export default MyApp

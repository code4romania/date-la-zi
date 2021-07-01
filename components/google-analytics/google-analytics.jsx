import Head from 'next/head'
import { canIncludeScript, GA_TRACKING_ID } from '../../utils/gtag'

const GoogleAnalytics = () =>
  canIncludeScript && (
    <Head>
      <script
        async
        src={`https://www.googletagmanager.com/gtag/js?id=${GA_TRACKING_ID}`}
      />
      <script
        dangerouslySetInnerHTML={{
          __html: `
            window.dataLayer = window.dataLayer || [];
            function gtag(){dataLayer.push(arguments);}
            gtag('js', new Date());
            gtag('config', '${GA_TRACKING_ID}', {
              page_path: window.location.pathname,
            });
          `,
        }}
      />
    </Head>
  )

export default GoogleAnalytics

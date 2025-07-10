import type { NextConfig } from 'next'

const isDev = process.env.NODE_ENV === 'development'

const nextConfig: NextConfig = {
  async rewrites() {
    if (isDev) {
      return [
        {
          source: '/api/:path*',
          destination: `${process.env.NEXT_PUBLIC_API_BASE}/v1/:path*`
        },
        {
          source: '/signalr/:path*',
          destination: `${process.env.NEXT_PUBLIC_API_BASE}/:path*`
        }
      ]
    }

    return []
  },
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'dev.24cases.ru'
      }
    ]
  },
  experimental: {
    turbo: {
      rules: {
        '*.svg': {
          loaders: ['@svgr/webpack'],
          as: '*.js'
        }
      }
    }
  },
  webpack(config) {
    config.module.rules.push({
      test: /\.svg$/i,
      use: ['@svgr/webpack']
    })

    return config
  }
}

export default nextConfig

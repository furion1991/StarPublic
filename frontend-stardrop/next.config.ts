import type { NextConfig } from 'next'

const isDev = process.env.NODE_ENV === 'development'

const nextConfig: NextConfig = {
  reactStrictMode: false,
  async rewrites() {
    const baseRewrites = [
      {
        source: '/verifelena.txt',
        destination: '/api/verifelena.txt'
      }
    ]

    if (isDev) {
      return [
        ...baseRewrites,
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

    return baseRewrites
  },
  images: {
    unoptimized: true,
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'dev.stardrop.app'
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

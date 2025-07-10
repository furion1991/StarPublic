import localFont from 'next/font/local'

export const fontExo = localFont({
  src: [
    {
      path: '../../../public/fonts/Exo-Light.ttf',
      weight: '300',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo-Regular.ttf',
      weight: '400',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo-Medium.ttf',
      weight: '500',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo-SemiBold.ttf',
      weight: '600',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo-Bold.ttf',
      weight: '700',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo-ExtraBold.ttf',
      weight: '800',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo-Black.ttf',
      weight: '900',
      style: 'normal'
    }
  ],
  variable: '--font-exo'
})

export const fontExo2 = localFont({
  src: [
    {
      path: '../../../public/fonts/Exo2-Light.ttf',
      weight: '300',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo2-Regular.ttf',
      weight: '400',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo2-Medium.ttf',
      weight: '500',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo2-SemiBold.ttf',
      weight: '600',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo2-Bold.ttf',
      weight: '700',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo2-ExtraBold.ttf',
      weight: '800',
      style: 'normal'
    },
    {
      path: '../../../public/fonts/Exo2-Black.ttf',
      weight: '900',
      style: 'normal'
    }
  ],
  variable: '--font-exo2'
})

export const fontRepublicaMinor = localFont({
  src: '../../../public/fonts/RepublicaMinor.ttf',
  variable: '--font-republica-minor'
})
